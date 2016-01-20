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
      private readonly string dir;
      private readonly string filePdfOutputName;
      private readonly string[] filesDwg;

      /// <summary>
      /// Сортировка листов
      /// </summary>
      public EnumLayoutsSort LayoutSort { get; set; }
            
      public enum EnumLayoutsSort
      {
         DatabaseOrder,
         TabOrder,
         LayoutNames
      }

      public PlotDirToPdf(string dir, string filePdfOutputName = "")
      {
         this.dir = dir;
         filesDwg = Directory.GetFiles(dir, "*.dwg", SearchOption.TopDirectoryOnly);
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
         DsdEntryCollection dsdCol = new DsdEntryCollection();         
         
         foreach (var fileDwg in filesDwg)
         {
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
                        var layout = entry.Value.GetObject(OpenMode.ForRead) as Layout;                        
                        DsdEntry dsdEntry = new DsdEntry();                        
                        dsdEntry.Layout = layout.LayoutName;
                        dsdEntry.DwgName = fileDwg;
                        //dsdEntry.Nps = "Setup1";
                        dsdEntry.NpsSourceDwg = fileDwg;
                        dsdEntry.Title = layout.LayoutName;
                        layouts.Add(new Tuple<Layout, DsdEntry>(layout, dsdEntry));
                        //dsdCol.Add(dsdEntry);
                     }
                  }
                  switch (LayoutSort)
                  {
                     case EnumLayoutsSort.DatabaseOrder:                        
                        break;
                     case EnumLayoutsSort.TabOrder:
                        layouts.Sort((l1, l2) => l1.Item1.TabOrder.CompareTo(l2.Item1.TabOrder));
                        break;
                     case EnumLayoutsSort.LayoutNames:
                        layouts.Sort((l1, l2) => l1.Item1.LayoutName.CompareTo(l2.Item1.LayoutName));
                        break;
                     default:
                        break;
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
      
      public static string GetLayoutSortName(EnumLayoutsSort layoutSort)
      {
         switch (layoutSort)
         {
            case EnumLayoutsSort.DatabaseOrder:
               return "Создание";
            case EnumLayoutsSort.TabOrder:
               return "Вкладки";
            case EnumLayoutsSort.LayoutNames:
               return "Имена";
            default:
               return "";
         }
      }
   }
}
