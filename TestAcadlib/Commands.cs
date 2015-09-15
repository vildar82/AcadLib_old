using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using AcadLib.Blocks;

[assembly: CommandClass(typeof (TestAcadlib.Commands))]

namespace TestAcadlib
{
    public class Commands
    {
      [CommandMethod("Test")]
      public void Test()
      {
         Document doc = AcAp.DocumentManager.MdiActiveDocument;
         Database db = doc.Database;
         Editor ed = doc.Editor;
         TypedValue[] filter = { new TypedValue(0, "INSERT") };
         PromptSelectionResult psr = ed.GetSelection(new SelectionFilter(filter));
         if (psr.Status != PromptStatus.OK) return;
         PromptPointResult ppr = ed.GetPoint("\nInsertion point: ");
         if (ppr.Status != PromptStatus.OK) return;
         using (Transaction tr = db.TransactionManager.StartTransaction())
         {
            System.Data.DataTable dataTable = psr.Value.GetObjectIds()
                .Select(id => new BlockAttribute(id.GetObject<BlockReference>()))
                .ToDataTable("Extraction");
            Table tbl = dataTable.ToAcadTable(9.0, 40.0);
            tbl.Position = ppr.Value.TransformBy(ed.CurrentUserCoordinateSystem);
            BlockTableRecord btr = db.CurrentSpaceId.GetObject<BlockTableRecord>(OpenMode.ForWrite);
            btr.AppendEntity(tbl);
            tr.AddNewlyCreatedDBObject(tbl, true);
            try
            {
               string filename = (string)AcAp.GetSystemVariable("dwgprefix") + "Extraction.xls";
               dataTable.WriteXls(filename, null, true);
            }
            catch
            {
               AcAp.ShowAlertDialog("Failed to open Excel");
            }
            tr.Commit();
         }
      }
   }
}
