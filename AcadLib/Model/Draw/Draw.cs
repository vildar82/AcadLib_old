using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    public static class Draw
    {
        public static void Polyline(Layers.LayerInfo layer = null, Color color=null, LineWeight? lineWeight = null,
                                        string lineType = null, double? lineTypeScale = null)
        {
            // Обертка запуска команды рисования полилинии с заданными свойствами.
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;           

            // Вызов команды рисования полилинии
            using (new DrawParameters(db, layer, color, lineWeight, lineType, lineTypeScale))
            {
                doc.Editor.Command("_PLINE");
            }            
        }
    }
}
