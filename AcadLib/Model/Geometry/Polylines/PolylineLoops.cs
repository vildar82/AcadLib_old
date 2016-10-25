using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Geometry
{
    public static class PolylineLoops
    {
        /// <summary>
        /// Точки "петли" полилинии между точками пересечения.
        /// </summary>
        /// <param name="contour">Исходная Полилиния</param>
        /// <param name="ptIntersect1">Первая точка петли (пересечения)</param>
        /// <param name="ptIntersect2">Вторая точка петли (пересечения)</param>
        /// <param name="above">Петля выше или ниже точек пересечения</param>
        /// <param name="includePtIntersects">Включать ли сами точки пересечения в результат</param>
        /// <returns>Список точек петли пересечения</returns>
        public static List<Point2d> GetLoopSideBetweenHorizontalIntersectPoints (this Polyline contour,
            Point3d ptIntersect1, Point3d ptIntersect2, bool above = true, bool includePtIntersects = true)
        {
            List<Point2d> pointsLoopAbove = new List<Point2d>();

            var ptIntersectStart = ptIntersect1;
            var ptIntersectEnd = ptIntersect2;           

            // Индекс стартовой точки петли (вершины) с нужной стороны от точки пересечения
            int dir;
            var indexStart = GetStartIndex(contour, ptIntersect1, above, out dir);
            int indexCur = indexStart;
            
            int dirEnd;
            var indexEnd = GetStartIndex(contour, ptIntersect2, above, out dirEnd);
            if (dir == 0)
            {
                dir = dirEnd;
                indexCur = indexEnd;
                indexEnd = indexStart;
                ptIntersectStart = ptIntersect2;
                ptIntersectEnd = ptIntersect1;
            }

            if (includePtIntersects)
                pointsLoopAbove.Add(ptIntersectStart.Convert2d());

            if (dir != 0)
            {
                if (indexCur == indexEnd)
                {
                    AddPoint(pointsLoopAbove, dir, ref indexCur, contour);
                }
                else
                {
                    while (indexCur != indexEnd)
                    {
                        AddPoint(pointsLoopAbove, dir, ref indexCur, contour);
                    }
                    // Добавление последней вершины
                    AddPoint(pointsLoopAbove, dir, ref indexCur, contour);
                }
            }            

            if (includePtIntersects)
                pointsLoopAbove.Add(ptIntersectEnd.Convert2d());

            return pointsLoopAbove;
        }

        private static void AddPoint (List<Point2d> pointsLoopAbove, int dir, ref int indexCur, Polyline contour)
        {
            var pt = contour.GetPoint2dAt(indexCur);
            pointsLoopAbove.Add(pt);
            indexCur += dir;
            if (indexCur == -1)
            {
                indexCur = contour.NumberOfVertices - 1;
            }
            else if (indexCur == contour.NumberOfVertices)
            {
                indexCur = 0;
            }
        }

        private static int GetStartIndex (Polyline contour, Point3d ptIntersect1, bool above,
            out int dir)
        {
            var param = contour.GetParameterAtPoint(ptIntersect1);
            int indexMin = (int)param;
            int indexMax = (int)Math.Ceiling(param);            
            var seg = contour.GetLineSegmentAt(indexMin);
            var indexStart = indexMax;
            if (indexMin == indexMax)
            {
                dir = 0;
            }
            else
            {
                dir = 1;
                if ((above && (seg.StartPoint.Y > seg.EndPoint.Y)) ||
                    (!above && (seg.StartPoint.Y < seg.EndPoint.Y)))
                {
                    indexStart = indexMin;
                    dir = -1;
                }
            }
            return indexStart;
        }
    }
}
