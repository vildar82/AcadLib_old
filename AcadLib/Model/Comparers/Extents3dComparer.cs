using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AcadLib.Comparers
{
    [PublicAPI]
    public class Extents3dComparer : IEqualityComparer<Extents3d>
    {
        public static Extents3dComparer Default1 { get; } = new Extents3dComparer(new Tolerance(0.1, 1));

        public Tolerance Tolerance { get; set; } = Tolerance.Global;

        public Extents3dComparer()
        {
        }

        public Extents3dComparer(Tolerance tolerance)
        {
            Tolerance = tolerance;
        }

        public bool Equals(Extents3d x, Extents3d y)
        {
            return x.IsEqualTo(y, Tolerance);
        }

        public int GetHashCode(Extents3d obj)
        {
            return obj.GetHashCode();
        }
    }
}