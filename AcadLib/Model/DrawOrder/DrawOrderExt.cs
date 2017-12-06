using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;

namespace AcadLib
{
    public static class DrawOrderExt
    {
        public static void DrawOrder([NotNull] this BlockTableRecord btr, ObjectId top, ObjectId bot)
        {
            var drawOrder = btr.DrawOrderTableId.GetObject(OpenMode.ForWrite) as DrawOrderTable;
            if (drawOrder == null) return;
            var idsAbove = new ObjectIdCollection { top };
            drawOrder.MoveAbove(idsAbove, bot);
        }
    }
}
