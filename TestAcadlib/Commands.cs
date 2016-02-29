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

         Inspector.AddError("1");
         Inspector.AddError("2");
         Inspector.AddError("3");
         Inspector.AddError("5");
         Inspector.AddError("26");

         Inspector.ShowDialog();
      }
   }
}
