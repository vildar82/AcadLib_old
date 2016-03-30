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

            var sel = ed.GetEntity("Выбери блок");
            using (var blRef = sel.ObjectId.Open( OpenMode.ForRead)as BlockReference)
            {
                using (var btr = blRef.BlockTableRecord.Open(OpenMode.ForRead) as BlockTableRecord)
                {
                    var image = AcadLib.Blocks.Visual.BlockPreviewHelper.GetPreviewImage(btr);
                }
            }
        }
    }
}
