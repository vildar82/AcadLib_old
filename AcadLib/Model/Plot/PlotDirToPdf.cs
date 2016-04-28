using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Publishing;

namespace AcadLib.Plot
{
    // http://adndevblog.typepad.com/autocad/2012/05/how-to-use-the-autodeskautocadpublishingpublisherpublishdsd-api-in-net.html
    public class PlotDirToPdf
    {
        private static Comparers.AlphanumComparator alphaComparer = Comparers.AlphanumComparator.New;
        private string dir;
        private string filePdfOutputName;
        private string[] filesDwg;                

        public PlotOptions Options { get; set; }

        public PlotDirToPdf(string dir, string filePdfOutputName = "")
        {
            filesDwg = Directory.GetFiles(dir, "*.dwg", SearchOption.TopDirectoryOnly);
            filesDwg = filesDwg.OrderBy(f => f, alphaComparer).ToArray();
            this.dir = dir;
            this.filePdfOutputName = filePdfOutputName == "" ? Path.GetFileName(dir) : filePdfOutputName;
        }

        public PlotDirToPdf(string[] filesDwg, string filePdfOutputName)
        {
            dir = Path.GetDirectoryName(filesDwg.First());
            this.filesDwg = filesDwg;
            this.filePdfOutputName = filePdfOutputName;
        }        

        public void Plot()
        {
            if (Options == null)
            {
                Options = new PlotOptions();
                PlotFiles();
            }
            else
            {
                if (!Options.OnePdfOrEachDwg)
                {
                    foreach (var file in filesDwg)
                    {
                        PlotFileToPdf(file);
                    }
                }
                else
                {
                    PlotFiles();
                }
            }
            // открыть проводник с файлом
            System.Diagnostics.Process.Start("explorer", dir);
        }

        private void PlotFileToPdf(string file)
        {
            dir = Path.GetDirectoryName(file);
            filePdfOutputName = Path.GetFileNameWithoutExtension(file) + ".pdf";            
            filesDwg = new[] { file};
            PlotFiles();
        }

        private void PlotFiles()
        {            
            DsdEntryCollection dsdCol = new DsdEntryCollection();

            int indexfile = 0;
            foreach (var fileDwg in filesDwg)
            {
                indexfile++;
                using (var dbTemp = new Database(false, true))
                {
                    dbTemp.ReadDwgFile(fileDwg, FileOpenMode.OpenForReadAndAllShare, false, "");
                    dbTemp.CloseInput(true);
                    using (var t = dbTemp.TransactionManager.StartTransaction())
                    {
                        List<Tuple<Layout, DsdEntry>> layouts = new List<Tuple<Layout, DsdEntry>>();
                        DBDictionary layoutDict = (DBDictionary)dbTemp.LayoutDictionaryId.GetObject(OpenMode.ForRead);
                        foreach (DBDictionaryEntry entry in layoutDict)
                        {
                            if (entry.Key != "Model")
                            {
                                if (!entry.Value.IsErased)
                                {                                    
                                    var layout = entry.Value.GetObject(OpenMode.ForRead) as Layout;
                                    DsdEntry dsdEntry = new DsdEntry();
                                    dsdEntry.Layout =layout.LayoutName;
                                    dsdEntry.DwgName = fileDwg;
                                    //dsdEntry.Nps = "Setup1";
                                    dsdEntry.NpsSourceDwg = fileDwg;
                                    dsdEntry.Title =indexfile + "-" + layout.LayoutName;
                                    layouts.Add(new Tuple<Layout, DsdEntry>(layout, dsdEntry));
                                    //dsdCol.Add(dsdEntry);
                                }
                            }
                        }
                        if (Options.SortTabOrName)
                        {
                            layouts.Sort((l1, l2) => l1.Item1.TabOrder.CompareTo(l2.Item1.TabOrder));
                        }                        
                        else
                        {
                            layouts.Sort((l1, l2) => l1.Item1.LayoutName.CompareTo(l2.Item1.LayoutName));
                        }                        
                        layouts.ForEach(l => dsdCol.Add(l.Item2));
                        t.Commit();
                    }
                }
            }
            PublisherDSD(dsdCol);            
        }

        public void PublisherDSD(DsdEntryCollection collection)
        {
            try
            {
                Application.SetSystemVariable("BACKGROUNDPLOT", 0);
                DsdData dsd = new DsdData();

                dsd.SetDsdEntryCollection(collection);

                //dsd.ProjectPath = dirOutput;
                dsd.LogFilePath = Path.Combine(dir, "logPlotPdf.log");
                dsd.SheetType = SheetType.MultiPdf;
                dsd.IsSheetSet = true;
                dsd.NoOfCopies = 1;
                dsd.DestinationName = Path.Combine(dir, filePdfOutputName);
                dsd.SheetSetName = "PublisherSet";
                dsd.PromptForDwfName = false;
                string dsdFile = Path.Combine(dir, "PublisherDsd.dsd");
                dsd.WriteDsd(dsdFile);

                int nbSheets = collection.Count;

                using (PlotProgressDialog progressDlg = new PlotProgressDialog(false, nbSheets, true))
                {
                    progressDlg.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Печать");
                    progressDlg.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Отмена задания");
                    progressDlg.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Отмена листа");
                    progressDlg.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Прогресс печати");
                    progressDlg.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Печать листа");

                    progressDlg.UpperPlotProgressRange = 100;
                    progressDlg.LowerPlotProgressRange = 0;

                    progressDlg.UpperSheetProgressRange = 100;
                    progressDlg.LowerSheetProgressRange = 0;

                    progressDlg.IsVisible = true;

                    Publisher publisher = Application.Publisher;
                    PlotConfigManager.SetCurrentConfig("DWG To PDF.pc3");

                    //Application.Publisher.AboutToBeginPublishing += new Autodesk.AutoCAD.Publishing.AboutToBeginPublishingEventHandler(Publisher_AboutToBeginPublishing);

                    //Application.Publisher.PublishExecute(dsd, PlotConfigManager.CurrentConfig);

                    publisher.PublishDsd(dsdFile, progressDlg);
                }
                File.Delete(dsdFile);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        //public static string GetLayoutSortName(EnumLayoutsSort layoutSort)
        //{
        //    switch (layoutSort)
        //    {
        //        case EnumLayoutsSort.DatabaseOrder:
        //            return "Создание";
        //        case EnumLayoutsSort.TabOrder:
        //            return "Вкладки";
        //        case EnumLayoutsSort.LayoutNames:
        //            return "Имена";
        //        default:
        //            return "";
        //    }
        //}
    }
}
