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
            Point3d ptIntersect1, Point3d ptIntersect2, bool above = true, bool includePtIntersects=true)
        {
            List<Point2d> pointsLoopAbove = new List<Point2d>();

            if (includePtIntersects)
                pointsLoopAbove.Add(ptIntersect1.Convert2d());

            int numVertex = contour.NumberOfVertices;

            // Индекс стартовой точки петли (вершины) с нужной стороны от точки пересечения
            int dir;
            var indexStart = GetStartIndex(contour, ptIntersect1, above, out dir);
            int indexCur = indexStart;

            // Добавление первой стартовой точки (вершины)
            AddPoint(pointsLoopAbove, dir, ref indexCur, contour);

            int dirEnd;
            var indexEnd = GetStartIndex(contour, ptIntersect2, above, out dirEnd);

            //bool isContinue = true;

            int countWhile = 0;
            if (indexStart != indexEnd)
            {
                while (indexCur != indexEnd)
                {
                    if (indexCur == -1)
                    {
                        indexCur = numVertex - 1;
                    }
                    else if (indexCur == numVertex)
                    {
                        indexCur = 0;
                    }
                    
                    AddPoint(pointsLoopAbove, dir, ref indexCur, contour);                    
                    countWhile++;
                }
                // Добавление последней стартовой точки (вершины)
                AddPoint(pointsLoopAbove, dir, ref indexEnd, contour);
            }
            if (includePtIntersects)
                pointsLoopAbove.Add(ptIntersect2.Convert2d());

            return pointsLoopAbove;
        }

        private static void AddPoint (List<Point2d> pointsLoopAbove, int dir, ref int indexCur, Polyline contour)
        {
            var pt = contour.GetPoint2dAt(indexCur);
            pointsLoopAbove.Add(pt);
            indexCur += dir;
        }

        private static int GetStartIndex (Polyline contour, Point3d ptIntersect1, bool above,
            out int dir)
        {
            var param = contour.GetParameterAtPoint(ptIntersect1);
            int indexMin = (int)param;
            int indexMax = (int)Math.Ceiling(param);
            var seg = contour.GetLineSegmentAt(indexMin);
            dir = 1;
            var indexStart = indexMax;
            if ((above && (seg.StartPoint.Y > seg.EndPoint.Y)) ||
                (!above && (seg.StartPoint.Y < seg.EndPoint.Y)))
            {
                indexStart = indexMin;
                dir = -1;
            }
            return indexStart;
        }
    }
}
