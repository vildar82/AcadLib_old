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

         Extents3d ext = new Extents3d ();
         var center = ext.Center();
      }
   }
}
