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
                CheckDublicateBlocks.Check();
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

            int[] paleNumbersInt = new[] { 1, 2, 3, 4, 5, 7, 8, 10, 15, 16, 100, 101, 102, 103, 105, 106, 107, 109 };
            var nums = MathExt.IntsToStringSequence(paleNumbersInt);
        }
    }
}
