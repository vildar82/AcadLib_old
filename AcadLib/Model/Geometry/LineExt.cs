using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JetBrains.Annotations;

namespace AcadLib.Geometry
{
    [PublicAPI]
    public static class LineExt
    {
        public static Point3d Center([NotNull] this Line line)
        {
            return line.StartPoint.Center(line.EndPoint);
        }
    }
}
