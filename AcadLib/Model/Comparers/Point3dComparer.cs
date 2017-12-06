using Autodesk.AutoCAD.Geometry;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace AcadLib.Comparers
{
    /// <summary>
    /// Сравнение точек с заданным допуском
    /// </summary>
    public class Point3dEqualityComparer : IEqualityComparer<Point3d>
    {
        public Tolerance Tolerance { get; set; } = Tolerance.Global;

        /// <summary>
        /// Допуск 1 мм.
        /// </summary>
        [NotNull]
        public static Point3dEqualityComparer Comparer1
        {
            get {
                return new Point3dEqualityComparer(1);
            }
        }

        /// <summary>
        /// С допуском поумолчанию - Global
        /// </summary>
        public Point3dEqualityComparer() { }

        /// <summary>
        /// Задается допуск для точек
        /// </summary>        
        public Point3dEqualityComparer(double equalPoint)
        {
            Tolerance = new Tolerance(Tolerance.Global.EqualVector, equalPoint);
        }

        public bool Equals(Point3d p1, Point3d p2)
        {
            return p1.IsEqualTo(p2, Tolerance);
        }

        public int GetHashCode(Point3d p)
        {
            return 0;
        }
    }
}
