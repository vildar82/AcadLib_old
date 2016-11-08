using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Geometry
{
    /// <summary>
    /// Усреднение вершин соседних полилиний.
    /// Только для полилиний с линейными сегментами.
    /// Усредняются вершины на обоих полилиниях
    /// </summary>
    public static class AverageVertexPolylines
    {
        /// <summary>
        /// Усреднение вершин
        /// </summary>
        /// <param name="pl">Первая полилиния</param>
        /// <param name="plOther">Вторая полилиния</param>
        /// <param name="tolerance">Определение совпадения вершин для их усреднения</param>
        public static void AverageVertexes(this Polyline pl,ref Polyline plOther, Tolerance tolerance)
        {
            var ptsOther = plOther.GetPoints();
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                var pt = pl.GetPoint2dAt(i);
                var nearestPtOther = ptsOther.Where(p => p.IsEqualTo(pt, tolerance));
                // усреднение вершин
                if (nearestPtOther.Any())
                {
                    // Средняя точка
                    var avaragePt = pt.Center(nearestPtOther.First());
                    pl.RemoveVertexAt(i);
                    pl.AddVertexAt(i, avaragePt, 0, 0, 0);

                    foreach (var item in nearestPtOther)
                    {
                        var index= ptsOther.IndexOf(item);
                        plOther.RemoveVertexAt(index);
                        plOther.AddVertexAt(index, avaragePt, 0, 0, 0);
                    }
                }
            }
        }
    }
}
