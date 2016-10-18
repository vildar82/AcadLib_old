using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Comparers
{
    /// <summary>
    /// Сравнение точек с заданным допуском
    /// </summary>
    public class Point2dEqualityComparer : IEqualityComparer<Point2d>
    {
        public Tolerance Tolerance { get; set; } = Tolerance.Global;

        /// <summary>
        /// Допуск 1.
        /// </summary>
        public static Point2dEqualityComparer Comparer1
        {
            get
            {
                return new Point2dEqualityComparer(1);
            }
        }

        /// <summary>
        /// С допуском поумолчанию - Global
        /// </summary>
        public Point2dEqualityComparer() { }

        /// <summary>
        /// Задается допуск для точек
        /// </summary>        
        public Point2dEqualityComparer(double equalPoint)
        {
            Tolerance = new Tolerance(Tolerance.Global.EqualVector, equalPoint);
        }

        public bool Equals(Point2d p1, Point2d p2)
        {
            return p1.IsEqualTo(p2, Tolerance);
        }

        public int GetHashCode(Point2d p)
        {
            return 0;
        }
    }
}
