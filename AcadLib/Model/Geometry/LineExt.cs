namespace AcadLib.Geometry
{
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.Geometry;
    using JetBrains.Annotations;

    [PublicAPI]
    public static class LineExt
    {
        public static Point3d Center([NotNull] this Line line)
        {
            return line.StartPoint.Center(line.EndPoint);
        }
    }
}