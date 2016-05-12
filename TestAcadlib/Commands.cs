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

            // Команда рисования полилинии с заданными свойствами.
            // свойства: слой, цвет, вес линии, тип линии, масштаб типа линии
            LayerInfo li = new LayerInfo("Test");
            li.Color = Color.FromColorIndex(ColorMethod.ByAci, 30);
            Draw.Polyline(li,color: li.Color, lineWeight: LineWeight.LineWeight053,
                        lineType: "Штрих-пунктирная с двумя точками", lineTypeScale: 53);
        }       
    }
}
