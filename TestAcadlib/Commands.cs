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

         Inspector.AddError("dfgdgdgsdf sdfg dsg ", icon: SystemIcons.Error);
         Inspector.AddError("dfgdgdgsdf sdfg dsg ", icon: SystemIcons.Error);
         Inspector.AddError("dfgdgdgsdf sdfg dsg ", icon: SystemIcons.Error);
         Inspector.AddError("dfgdgdgsdf sdfg dsg ", icon: SystemIcons.Error);
         Inspector.AddError("dfgdgdgsdf sdfg dsg ", icon: SystemIcons.Error);
         Inspector.AddError("dfgdgdgsdf sdfg dsg ", icon: SystemIcons.Error);
         Inspector.AddError("dfgdgdgsdf sdfg dsg ", icon: SystemIcons.Error);
         Inspector.AddError("dfgdgdgsdf sdfg dsg ", icon: SystemIcons.Error);

         Inspector.Show();
      }
   }
}
