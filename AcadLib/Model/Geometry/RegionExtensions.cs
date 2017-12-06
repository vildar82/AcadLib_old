using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JetBrains.Annotations;

namespace AcadLib.Geometry
{
    /// <summary>
    /// Provides extension methods for the Region type.
    /// </summary>
    public static class RegionExtensions
    {
        /// <summary>
        /// Gets the centroid of the region.
        /// </summary>
        /// <param name="reg">The instance to which the method applies.</param>
        /// <returns>The centroid of the region (WCS coordinates).</returns>
        public static Point3d Centroid([NotNull] this Region reg)
        {
            using (var sol = new Solid3d())
            {
                sol.Extrude(reg, 2.0, 0.0);
                return sol.MassProperties.Centroid - reg.Normal;
            }
        }
    }
}
