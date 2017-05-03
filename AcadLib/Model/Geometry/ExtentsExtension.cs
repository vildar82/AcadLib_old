using Autodesk.AutoCAD.Geometry;

namespace Autodesk.AutoCAD.DatabaseServices
{
    public static class ExtentsExtension
    {
        public static Extents3d Convert3d(this Extents2d ext)
        {
            return new Extents3d(ext.MinPoint.Convert3d(), ext.MaxPoint.Convert3d());
        }

        public static Extents2d Convert2d(this Extents3d ext)
        {
            return new Extents2d(ext.MinPoint.Convert2d(), ext.MaxPoint.Convert2d());
        }

        public static Polyline GetPolyline(this Extents2d ext)
        {
            return ext.Convert3d().GetPolyline();
        }

        public static Polyline GetPolyline(this Extents3d ext)
        {
            var pl = new Polyline();
            pl.AddVertexAt(0, ext.MinPoint.Convert2d(), 0, 0, 0);
            pl.AddVertexAt(1, new Point2d(ext.MinPoint.X, ext.MaxPoint.Y), 0, 0, 0);
            pl.AddVertexAt(2, ext.MaxPoint.Convert2d(), 0, 0, 0);
            pl.AddVertexAt(3, new Point2d(ext.MaxPoint.X, ext.MinPoint.Y), 0, 0, 0);
            pl.Closed = true;
            return pl;
        }

        /// <summary>
        /// Определение точки центра границы Extents3d
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static Point3d Center(this Extents3d ext)
        {
            return new Point3d((ext.MaxPoint.X + ext.MinPoint.X) * 0.5,
                                (ext.MaxPoint.Y + ext.MinPoint.Y) * 0.5, 0);
        }

        /// <summary>
        /// Длина диагонали границ (расстояние между точками MaxPoint и MinPoint)
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static double Diagonal(this Extents3d ext)
        {
            return (ext.MaxPoint - ext.MinPoint).Length;
        }

        public static double Diagonal(this Extents2d ext)
        {
            return (ext.MaxPoint - ext.MinPoint).Length;
        }

        /// <summary>
        /// Попадает ли точка внутрь границы
        /// </summary>      
        /// <returns></returns>
        public static bool IsPointInBounds(this Extents3d ext, Point3d pt)
        {
            var res = false;
            if (pt.X > ext.MinPoint.X && pt.Y > ext.MinPoint.Y &&
               pt.X < ext.MaxPoint.X && pt.Y < ext.MaxPoint.Y)
            {
                res = true;
            }
            return res;
        }

        /// <summary>
        /// Попадает ли точка внутрь границы
        /// </summary>      
        /// <returns></returns>
        public static bool IsPointInBounds(this Extents3d ext, Point3d pt, double tolerance)
        {
            var res = false;
            if (pt.X > (ext.MinPoint.X - tolerance) && pt.Y > (ext.MinPoint.Y - tolerance) &&
               pt.X < (ext.MaxPoint.X + tolerance) && pt.Y < (ext.MaxPoint.Y + tolerance))
            {
                res = true;
            }
            return res;
        }
    }
}
