using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    public static class DrawOrderExt
    {
        public static void DrawOrder(this BlockTableRecord btr,  ObjectId top, ObjectId bot)
        {
            var drawOrder = btr.DrawOrderTableId.GetObject(OpenMode.ForWrite) as DrawOrderTable;
            if (drawOrder == null) return;
            var idsAbove = new ObjectIdCollection {top};
            drawOrder.MoveAbove(idsAbove,bot);
        }
    }
}
