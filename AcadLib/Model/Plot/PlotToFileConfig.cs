using System.Collections.Generic;
using System.IO;
using System.Text;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.PlottingServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Gile.Publish
{
    // Base class for the different configurations
    public abstract class PlotToFileConfig
    {
        // Private fields
        private string dsdFile, dwgFile, outputDir, outputFile, plotType;
        private int sheetNum;
        private IEnumerable<Layout> layouts;
        private const string LOG = "publish.log";

        // Base constructor
        public PlotToFileConfig(string outputFile, IEnumerable<Layout> layouts, string plotType)
        {
            var db = HostApplicationServices.WorkingDatabase;
            dwgFile = db.Filename;
            outputDir = Path.GetDirectoryName(outputFile);
            dsdFile = Path.ChangeExtension(outputFile, "dsd");
            this.layouts = layouts;
            this.plotType = plotType;
            var ext = plotType == "0" || plotType == "1" ? "dwf" : "pdf";
            this.outputFile = Path.Combine(outputDir, Path.ChangeExtension(Path.GetFileName(outputFile), ext));
        }

        // Plot the layouts
        public void Publish()
        {
            if (TryCreateDSD())
            {
                var bgp = Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("BACKGROUNDPLOT");
                var ctab = Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("CTAB");
                try
                {
                    Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("BACKGROUNDPLOT", 0);

                    var publisher = Autodesk.AutoCAD.ApplicationServices.Core.Application.Publisher;
                    var plotDlg = new PlotProgressDialog(false, sheetNum, false);
                    publisher.PublishDsd(dsdFile, plotDlg);
                    plotDlg.Destroy();
                }
                catch (System.Exception exn)
                {
                    var ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;                    
                    ed.WriteMessage("\nError: {0}\n{1}", exn.Message, exn.StackTrace);
                    throw;
                }
                finally
                {
                    Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("BACKGROUNDPLOT", bgp);
                    Application.SetSystemVariable("CTAB", ctab);
                    File.Delete(dsdFile);
                }
            }
        }

        // Creates the DSD file from a template (default options)
        private bool TryCreateDSD()
        {
            using (var dsd = new DsdData())
            using (var dsdEntries = CreateDsdEntryCollection(layouts))
            {
                if (dsdEntries == null || dsdEntries.Count <= 0) return false;

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }
                sheetNum = dsdEntries.Count;

                dsd.SetDsdEntryCollection(dsdEntries);

                dsd.SetUnrecognizedData("PwdProtectPublishedDWF", "FALSE");
                dsd.SetUnrecognizedData("PromptForPwd", "FALSE");
                dsd.NoOfCopies = 1;
                dsd.DestinationName = outputFile;
                dsd.IsHomogeneous = false;
                dsd.LogFilePath = Path.Combine(outputDir, LOG);

                PostProcessDSD(dsd);

                return true;
            }
        }

        // Creates an entry collection (one per layout) for the DSD file
        private DsdEntryCollection CreateDsdEntryCollection(IEnumerable<Layout> layouts)
        {
            var entries = new DsdEntryCollection();
            foreach (var layout in layouts)
            {
                var dsdEntry = new DsdEntry()
                {
                    DwgName = dwgFile,
                    Layout = layout.LayoutName,
                    Title = Path.GetFileNameWithoutExtension(dwgFile) + "-" + layout.LayoutName,
                    Nps = layout.TabOrder.ToString()
                };
                entries.Add(dsdEntry);
            }
            return entries;
        }

        // Writes the definitive DSD file from the templates and additional informations
        private void PostProcessDSD(DsdData dsd)
        {
            string str, newStr;
            var tmpFile = Path.Combine(outputDir, "temp.dsd");

            try
            {
                dsd.WriteDsd(tmpFile);

                using (var reader = new StreamReader(tmpFile, Encoding.Default))
                using (var writer = new StreamWriter(dsdFile, false, Encoding.Default))
                {
                    while (!reader.EndOfStream)
                    {
                        str = reader.ReadLine();
                        if (str.Contains("Has3DDWF"))
                        {
                            newStr = "Has3DDWF=0";
                        }
                        else if (str.Contains("OriginalSheetPath"))
                        {
                            newStr = "OriginalSheetPath=" + dwgFile;
                        }
                        else if (str.Contains("Type"))
                        {
                            newStr = "Type=" + plotType;
                        }
                        else if (str.Contains("OUT"))
                        {
                            newStr = "OUT=" + outputDir;
                        }
                        else if (str.Contains("IncludeLayer"))
                        {
                            newStr = "IncludeLayer=TRUE";
                        }
                        else if (str.Contains("PromptForDwfName"))
                        {
                            newStr = "PromptForDwfName=FALSE";
                        }
                        else if (str.Contains("LogFilePath"))
                        {
                            newStr = "LogFilePath=" + Path.Combine(outputDir, LOG);
                        }
                        else
                        {
                            newStr = str;
                        }
                        writer.WriteLine(newStr);
                    }
                }
            }
            catch
            {

            }
            File.Delete(tmpFile);
        }
    }

    // Class to plot one DWF file per sheet
    public class SingleSheetDwf : PlotToFileConfig
    {
        public SingleSheetDwf(string outputFile, IEnumerable<Layout> layouts)
            : base(outputFile, layouts, "0")
        {

        }
    }

    // Class to plot a multi-sheet DWF file
    public class MultiSheetsDwf : PlotToFileConfig
    {
        public MultiSheetsDwf(string outputFile, IEnumerable<Layout> layouts)
            : base(outputFile, layouts, "1")
        {

        }
    }

    // Class to plot one PDF file per sheet
    public class SingleSheetPdf : PlotToFileConfig
    {
        public SingleSheetPdf(string outputFile, IEnumerable<Layout> layouts)
            : base(outputFile, layouts, "5")
        {

        }
    }

    // Class to plot a multi-sheet PDF file
    public class MultiSheetsPdf : PlotToFileConfig
    {
        public MultiSheetsPdf(string outputFile, IEnumerable<Layout> layouts)
            : base(outputFile, layouts, "6")
        {

        }
    }
}