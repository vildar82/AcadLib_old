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
using AcadLib.Jigs;
using Autodesk.AutoCAD.Colors;

namespace TestAcadlib
{
    public class Commands
    {
        [CommandMethod("Test")]
        public void Test()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            var tables = new List<Entity>();
            for (int i = 0; i < 10; i++)
            {
                var table = new Table();
                table.SetSize(i, i * 2);
                table.Cells[0, 0].TextString = i.ToString();
                table.GenerateLayout();
                tables.Add(table);                
            }           

            ed.Drag(tables, 50);
        }       
    }
}
