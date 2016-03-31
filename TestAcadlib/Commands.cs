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
using Autodesk.AutoCAD.Geometry;
using AcadLib.Extensions;
using AcadLib.Errors;
using System.Drawing;
using AcadLib.Blocks.Dublicate;
using AcadLib.Blocks.Visual;
using AcadLib;

[assembly: CommandClass(typeof (TestAcadlib.Commands))]

namespace TestAcadlib
{
    public class Commands
    {
        [CommandMethod("TestDublic")]
        public void TestDublic()
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            try
            {
                CheckDublicateBlocks.Tolerance = new Tolerance(0.02, 15);
                CheckDublicateBlocks.Check(new HashSet<string>() { "RV_EL_BS_Базовая стена" });                
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);
            }
        }

        [CommandMethod("Test")]
        public void Test()
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            Table table = new Table();
            table.SetDatabaseDefaults();
            table.SetSize(5, 5);
            table.SetBorders(LineWeight.LineWeight050);

            table.Cells[0, 0].TextString = "Название";
            var rowHead = table.Rows[1];
            int count = 1;
            foreach (var item in rowHead)
            {
                table.Cells[item.Row, item.Column].TextString = "Заголовок-" + count++;
            }
            count = 1;
            for (int i = 2; i < table.Rows.Count; i++)
            {                
                table.Cells[i, 0].TextString = "Дата " + i + count++;
                table.Cells[i, 1].TextString = "Дата " + i + count++;
                table.Cells[i, 2].TextString = "Дата " + i + count++;
                table.Cells[i, 3].TextString = "Дата " + i + count++;
                table.Cells[i, 4].TextString = "Дата " + i + count++;
            }

            using (var t = db.TransactionManager.StartTransaction())
            {
                var ms = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                ms.AppendEntity(table);
                t.AddNewlyCreatedDBObject(table, true);
                t.Commit();
            }
        }

        [CommandMethod("CleanZombieBlocks", CommandFlags.Modal)]
        public void CleanZombieBlocks()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Database db = doc.Database;
            try
            {
                db.CleanZombieBlock();
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage($"\nОшибка - {ex.Message}");
            }
        }
    }
}
