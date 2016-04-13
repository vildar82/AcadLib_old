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
    public class Point3dEqualityComparer : IEqualityComparer<Point3d>
    {
        public Tolerance Tolerance { get; set; } = Tolerance.Global;

        /// <summary>
        /// Допуск 1 мм.
        /// </summary>
        public static Point3dEqualityComparer Comparer1
        {
            get
            {
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
            return p.GetHashCode();
        }
    }
}
