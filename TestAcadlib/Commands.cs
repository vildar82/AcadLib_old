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
using AcadLib.Blocks.Dublicate;
using AcadLib.Blocks.Visual;
using AcadLib;
using AcadLib.Layers;
using Autodesk.AutoCAD.Colors;

namespace TestAcadlib
{
    public class Commands
    {
        [CommandMethod("Test")]
        public void Test()
        {
            Inspector.AddError("fgdfg");
            var ress = Inspector.ShowDialog();
        }       
    }
}
