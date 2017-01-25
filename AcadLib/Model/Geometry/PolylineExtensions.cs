using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using AcadLib.Scale;
using Autodesk.AutoCAD.Colors;

namespace AcadLib.Geometry
{
    /// <summary>
    /// Enumeration of offset side options
    /// </summary>
    public enum OffsetSide { In, Out, Left, Right, Both }
    /// <summary>
    /// Provides extension methods for the Polyline type.
    /// </summary>
    public static class PolylineExtensions
    {
        /// <summary>
        /// GetParameterAtPoint - или попытка корректировки точки с помощью GetClosestPointTo и вызов для скорректированной точки GetParameterAtPoint
        /// </summary>
        /// <param name="pl">Полилиния</param>
        /// <param name="pt">Точка</param>
        /// <param name="extend">Input whether or not to extend curve in search for nearest point.</param>
        /// <returns></returns>
        public static double GetParameterAtPointTry(this Polyline pl, Point3d pt, bool extend = false)
        {
            try
            {
                return pl.GetParameterAtPoint(pt);
            }
            catch
            {
                var ptCorrect = pl.GetClosestPointTo(pt, extend);
                return pl.GetParameterAtPoint(ptCorrect);
            }
        }

        /// <summary>
        /// Подписывание вершин полилинии
        /// </summary>        
        public static void TestDrawVertexNumbers(this Polyline pl, Color color)
        {
            var scale =ScaleHelper.GetCurrentAnnoScale(HostApplicationServices.WorkingDatabase);
            var texts = new List<Entity>();
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                var text = new DBText();
                text.TextString = i.ToString();
                text.Position = pl.GetPoint2dAt(i).Convert3d();
                text.Height = 2.5 * scale;
                text.Color = color;
                texts.Add(text);                
            }
            EntityHelper.AddEntityToCurrentSpace(texts);
        }

        /// <summary>
        /// Прополка полилинии
        /// </summary>        
        public static void Wedding (this Polyline pl, Tolerance tolerance)
        {
            var count = pl.NumberOfVertices;
            for (int i = 1; i < count; i++)
            {
                int iPrew;
                int iCur;
                int iNext;
                iPrew = i-1;
                iCur = i;
                iNext = i + 1;
                if (iNext == count)
                {
                    break;
                }
                var prew = pl.GetPoint2dAt(iPrew);
                var cur = pl.GetPoint2dAt(iCur);
                var next = pl.GetPoint2dAt(iNext);                                
                if (IsPointsOnSomeLine(prew, cur, next, tolerance))
                {
                    pl.RemoveVertexAt(i);
                    i--;
                    count--;
                }
            }
            
            count = pl.NumberOfVertices;

            // Если начальная точка совпадает с конечной, то проверка сегменов до и после
            if (count > 3)
            {
                Point2d fp = pl.GetPoint2dAt(0);
                Point2d lp = pl.GetPoint2dAt(count - 1);
                Point2d next = pl.GetPoint2dAt(1);                
                if (fp.IsEqualTo(lp, tolerance))
                {
                    var cur = fp;
                    var prew = pl.GetPoint2dAt(count - 2);                                        
                    if (IsPointsOnSomeLine(prew, cur, next, tolerance))
                    {
                        pl.RemoveVertexAt(count - 1);
                        pl.RemoveVertexAt(0);
                        if (!pl.Closed)
                        {
                            pl.Closed = true;
                        }
                    }
                }
                else if (pl.Closed)
                {
                    var prew = lp;
                    var cur = fp;
                    if (IsPointsOnSomeLine(prew, cur, next, tolerance))
                    {
                        pl.RemoveVertexAt(0);                        
                    }
                }
            }
        }

        private static bool IsPointsOnSomeLine(Point2d pt1, Point2d pt2, Point2d pt3, Tolerance tolerance)
        {
            var vec1 = pt2 - pt1;
            var vec2 = pt3 - pt1;
            return vec1.IsParallelTo(vec2, tolerance);            
        }

        public static List<Point2d> GetPoints (this Polyline pl)
        {
            List<Point2d> points = new List<Point2d>();
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                points.Add(pl.GetPoint2dAt(i));
            }
            return points;
        }

        public static IEnumerable<Point2d> EnumeratePoints(this Polyline pl)
        {            
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                yield return pl.GetPoint2dAt(i);
            }            
        }        

        /// <summary>
        /// Breaks the polyline at specified point.
        /// </summary>
        /// <param name="pline">The polyline this method applies to.</param>
        /// <param name="brkPt">The point where to break the polyline.</param>
        /// <returns>An array of the two resullting polylines.</returns>
        public static Polyline[] BreakAtPoint (this Polyline pline, Point3d brkPt)
        {
            brkPt = pline.GetClosestPointTo(brkPt, false);

            // le point spécifié est sur le point de départ de la polyligne
            if (brkPt.IsEqualTo(pline.StartPoint))
                return new Polyline[2] { null, (Polyline)pline.Clone() };

            // le point spécifié est sur le point de fin de la polyligne
            if (brkPt.IsEqualTo(pline.EndPoint))
                return new Polyline[2] { (Polyline)pline.Clone(), null };

            double param = pline.GetParameterAtPoint(brkPt);
            int index = (int)param;
            int num = pline.NumberOfVertices;
            Polyline pl1 = (Polyline)pline.Clone();
            if (pline.Closed)
            {
                pl1.AddVertexAt(
                    pline.NumberOfVertices,
                    pline.GetPoint2dAt(0),
                    pline.GetStartWidthAt(num - 1),
                    pline.GetEndWidthAt(num - 1),
                    pline.GetBulgeAt(num - 1));
                pl1.Closed = false;
            }
            Polyline pl2 = (Polyline)pl1.Clone();

            // le point spécifié est sur un sommet de la polyligne
            if (Math.Round(param, 6) == index)
            {
                for (int i = pl1.NumberOfVertices - 1; i > index; i--)
                {
                    pl1.RemoveVertexAt(i);
                }
                for (int i = 0; i < index; i++)
                {
                    pl2.RemoveVertexAt(0);
                }
                return new Polyline[2] { pl1, pl2 };
            }

            // le point spécifié est sur un segment
            Point2d pt = brkPt.Convert2d(new Plane(Point3d.Origin, pline.Normal));
            for (int i = pl1.NumberOfVertices - 1; i > index + 1; i--)
            {
                pl1.RemoveVertexAt(i);
            }
            pl1.SetPointAt(index + 1, pt);
            for (int i = 0; i < index; i++)
            {
                pl2.RemoveVertexAt(0);
            }
            pl2.SetPointAt(0, pt);
            if (pline.GetBulgeAt(index) != 0.0)
            {
                double bulge = pline.GetBulgeAt(index);
                pl1.SetBulgeAt(index, MultiplyBulge(bulge, param - index));
                pl2.SetBulgeAt(0, MultiplyBulge(bulge, index + 1 - param));
            }
            return new Polyline[2] { pl1, pl2 };
        }

        /// <summary>
        /// Gets the centroid of the polyline.
        /// </summary>
        /// <param name="pl">The instance to which the method applies.</param>
        /// <returns>The centroid of the polyline (OCS coordinates).</returns>
        public static Point2d Centroid2d (this Polyline pl)
        {
            Point2d cen = new Point2d();
            Triangle2d tri = new Triangle2d();
            CircularArc2d arc = new CircularArc2d();
            double tmpArea;
            double area = 0.0;
            int last = pl.NumberOfVertices - 1;
            Point2d p0 = pl.GetPoint2dAt(0);
            double bulge = pl.GetBulgeAt(0);

            if (bulge != 0.0)
            {
                arc = pl.GetArcSegment2dAt(0);
                area = arc.AlgebricArea();
                cen = arc.Centroid() * area;
            }
            for (int i = 1; i < last; i++)
            {
                tri.Set(p0, pl.GetPoint2dAt(i), pl.GetPoint2dAt(i + 1));
                tmpArea = tri.AlgebricArea;
                cen += (tri.Centroid * tmpArea).GetAsVector();
                area += tmpArea;
                bulge = pl.GetBulgeAt(i);
                if (bulge != 0.0)
                {
                    arc = pl.GetArcSegment2dAt(i);
                    tmpArea = arc.AlgebricArea();
                    area += tmpArea;
                    cen += (arc.Centroid() * tmpArea).GetAsVector();
                }
            }
            bulge = pl.GetBulgeAt(last);
            if ((bulge != 0.0) && (pl.Closed == true))
            {
                arc = pl.GetArcSegment2dAt(last);
                tmpArea = arc.AlgebricArea();
                area += tmpArea;
                cen += (arc.Centroid() * tmpArea).GetAsVector();
            }
            return cen.DivideBy(area);
        }

        /// <summary>
        /// Gets the centroid of the polyline.
        /// </summary>
        /// <param name="pl">The instance to which the method applies.</param>
        /// <returns>The centroid of the polyline (WCS coordinates).</returns>
        public static Point3d Centroid (this Polyline pl)
        {
            return pl.Centroid2d().Convert3d(pl.Normal, pl.Elevation);
        }

        /// <summary>
        /// Adds an arc (fillet), if able, at each polyline vertex.
        /// </summary>
        /// <param name="pline">The instance to which the method applies.</param>
        /// <param name="radius">The arc radius.</param>
        public static void FilletAll (this Polyline pline, double radius)
        {
            int n = pline.Closed ? 0 : 1;
            for (int i = n; i < pline.NumberOfVertices - n; i += 1 + pline.FilletAt(i, radius))
            { }
        }

        /// <summary>
        /// Adds an arc (fillet) at the specified vertex.
        /// </summary>
        /// <param name="pline">The instance to which the method applies.</param>
        /// <param name="index">The index of the vertex.</param>
        /// <param name="radius">The arc radius.</param>
        /// <returns>1 if the operation succeeded, 0 if it failed.</returns>
        public static int FilletAt (this Polyline pline, int index, double radius)
        {
            int prev = index == 0 && pline.Closed ? pline.NumberOfVertices - 1 : index - 1;
            if (pline.GetSegmentType(prev) != SegmentType.Line ||
                pline.GetSegmentType(index) != SegmentType.Line)
            {
                return 0;
            }
            LineSegment2d seg1 = pline.GetLineSegment2dAt(prev);
            LineSegment2d seg2 = pline.GetLineSegment2dAt(index);
            Vector2d vec1 = seg1.StartPoint - seg1.EndPoint;
            Vector2d vec2 = seg2.EndPoint - seg2.StartPoint;
            double angle = (Math.PI - vec1.GetAngleTo(vec2)) / 2.0;
            double dist = radius * Math.Tan(angle);
            if (dist == 0.0 || dist > seg1.Length || dist > seg2.Length)
            {
                return 0;
            }
            Point2d pt1 = seg1.EndPoint + vec1.GetNormal() * dist;
            Point2d pt2 = seg2.StartPoint + vec2.GetNormal() * dist;
            double bulge = Math.Tan(angle / 2.0);
            if (Clockwise(seg1.StartPoint, seg1.EndPoint, seg2.EndPoint))
            {
                bulge = -bulge;
            }
            pline.AddVertexAt(index, pt1, bulge, 0.0, 0.0);
            pline.SetPointAt(index + 1, pt2);
            return 1;
        }

        /// <summary>
        /// Evaluates if the points are clockwise.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <returns>True if points are clockwise, False otherwise.</returns>
        private static bool Clockwise (Point2d p1, Point2d p2, Point2d p3)
        {
            return ((p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X)) < 1e-8;
        }

        /// <summary>
        /// Creates a new Polyline that is the result of projecting the Polyline parallel to 'direction' onto 'plane' and returns it.
        /// </summary>
        /// <param name="pline">The polyline to project.</param>
        /// <param name="plane">The plane onto which the curve is to be projected.</param>
        /// <param name="direction">Direction (in WCS coordinates) of the projection.</param>
        /// <returns>The projected Polyline.</returns>
        public static Polyline GetProjectedPolyline (this Polyline pline, Plane plane, Vector3d direction)
        {
            Tolerance tol = new Tolerance(1e-9, 1e-9);
            if (plane.Normal.IsPerpendicularTo(direction, tol))
                return null;

            if (pline.Normal.IsPerpendicularTo(direction, tol))
            {
                Plane dirPlane = new Plane(Point3d.Origin, direction);
                if (!pline.IsWriteEnabled) pline.UpgradeOpen();
                pline.TransformBy(Matrix3d.WorldToPlane(dirPlane));
                Extents3d extents = pline.GeometricExtents;
                pline.TransformBy(Matrix3d.PlaneToWorld(dirPlane));
                return GeomExt.ProjectExtents(extents, plane, direction, dirPlane);
            }

            return GeomExt.ProjectPolyline(pline, plane, direction);
        }

        /// <summary>
        /// Creates a new Polyline that is the result of projecting the curve along the given plane.
        /// </summary>
        /// <param name="pline">The polyline to project.</param>
        /// <param name="plane">The plane onto which the curve is to be projected.</param>
        /// <returns>The projected polyline</returns>
        public static Polyline GetOrthoProjectedPolyline (this Polyline pline, Plane plane)
        {
            return pline.GetProjectedPolyline(plane, plane.Normal);
        }

        /// <summary>
        /// Applies a factor to a polyline bulge.
        /// </summary>
        /// <param name="bulge">The bulge this method applies to.</param>
        /// <param name="factor">the factor to apply to the bulge.</param>
        /// <returns>The new bulge.</returns>
        public static double MultiplyBulge (double bulge, double factor)
        {
            return Math.Tan(Math.Atan(bulge) * factor);
        }

        private struct Point
        {
            public double X, Y;
        }

        [Obsolete("Используй новую перегрузку с параметром допуска, она работает быстрее.")]
        public static bool IsPointOnPolyline (this Polyline pl, Point3d pt)
        {
            bool isOn = false;            
            Point3d ptZeroZ = new Point3d(pt.X, pt.Y, pl.Elevation);
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                Curve3d seg = null;

                SegmentType segType = pl.GetSegmentType(i);
                if (segType == SegmentType.Arc)
                    seg = pl.GetArcSegmentAt(i);
                else if (segType == SegmentType.Line)
                    seg = pl.GetLineSegmentAt(i);

                if (seg != null)
                {
                    isOn = seg.IsOn(ptZeroZ);
                    if (isOn)
                        break;
                }
            }
            return isOn;
        }

        /// <summary>
        /// Лежит ли точка на полилинии.
        /// Через GetClosestPointTo().
        /// </summary>
        /// <param name="pl">Полилиния</param>
        /// <param name="pt">Точка</param>
        /// <param name="gap">Допуск. Если 0, то используется Tolerance.Global</param>        
        public static bool IsPointOnPolyline(this Polyline pl, Point3d pt, double gap)
        {
            var ptPl = pl.GetClosestPointTo(pt, false);            
            var tolerance = gap==0? Tolerance.Global: new Tolerance(gap, gap);
            return pt.IsEqualTo(ptPl, tolerance);
        }

        /// <summary>
        /// Попадает ли точка внутрь полигона (все линейные сегменты)
        /// Но, работает быстрее, чем IsPointInsidePolyline. Примерно в 10раз.
        /// </summary>      
        [Obsolete("Используй IsPointInsidePolyline(), подходящий для дуговых полилиний.")]
        public static bool IsPointInsidePolygon (this Polyline polygon, Point3d pt)
        {
            int n = polygon.NumberOfVertices;
            double angle = 0;
            Point pt1, pt2;
            for (int i = 0; i < n; i++)
            {
                pt1.X = polygon.GetPoint2dAt(i).X - pt.X;
                pt1.Y = polygon.GetPoint2dAt(i).Y - pt.Y;
                pt2.X = polygon.GetPoint2dAt((i + 1) % n).X - pt.X;
                pt2.Y = polygon.GetPoint2dAt((i + 1) % n).Y - pt.Y;
                angle += Angle2D(pt1.X, pt1.Y, pt2.X, pt2.Y);
            }
            return !(Math.Abs(angle) < Math.PI);                
        }

        /// <summary>
        /// Попадает ли точка внутрь полилинии. 
        /// Предполагается, что полилиния имеет замкнутый контур.
        /// Подходит для полилиний с дуговыми сегментами.
        /// Работает медленнее чем IsPointInsidePolygon(), примерно в 10 раз.
        /// </summary>        
        /// <param name="onIsInside">Если исходная точка лежит на полилинии, то считать, что она внутри или нет: True - внутри, False - снаружи.</param>
        /// <exception cref="Exceptions.ErrorException">Не удалось определить за несколько попыток.</exception>
        public static bool IsPointInsidePolyline(this Polyline pl, Point3d pt, bool onIsInside = false)
        {
            using (var ray = new Ray())
            {
                ray.BasePoint = pt;
                var vec = new Vector3d(0, 1, pt.Z);
                ray.SecondPoint = pt + vec;
                using (var ptsIntersects = new Point3dCollection())
                {
                    bool isContinue = false;
                    bool isPtOnPolyline = false;
                    int countWhile = 0;
                    do
                    {
                        using (var plane = new Plane())
                        {
                            pl.IntersectWith(ray, Intersect.OnBothOperands, plane, ptsIntersects, IntPtr.Zero, IntPtr.Zero);
                        }
                        isContinue = ptsIntersects.Cast<Point3d>().Any(p =>
                        {
                            if (pt.IsEqualTo(p))
                            {
                                isPtOnPolyline = true;
                                return true;
                            }
                            var param = pl.GetParameterAtPoint(p);
                            return param % 1 == 0;
                        });

                        if (isPtOnPolyline)
                        {
                            return onIsInside;
                        }

                        if (isContinue)
                        {
                            vec = vec.RotateBy(0.01, Vector3d.ZAxis);
                            ray.SecondPoint = pt + vec;
                            ptsIntersects.Clear();
                            countWhile++;
                            if (countWhile > 3)
                            {
                                throw new Exceptions.ErrorException(new Errors.Error (
                                    "Не определено попадает ли точка внутрь полилинии.", 
                                    pt.GetRectangleFromCenter(3), Matrix3d.Identity, 
                                    System.Drawing.SystemIcons.Error));
                            }
                        }
                    } while (isContinue);
                    return ptsIntersects.Count.IsOdd();
                }
            }
        }

        public static Polyline CreatePolyline (this List<Point2d> pts)
        {
            var pl = new Polyline();
            pts = pts.DistinctPoints();
            for (int i = 0; i < pts.Count; i++)
            {
                pl.AddVertexAt(i, pts[i], 0, 0, 0);
            }
            pl.Closed = true;
            return pl;
        }

        private static double Angle2D (double x1, double y1, double x2, double y2)
        {
            double dtheta, theta1, theta2;

            theta1 = Math.Atan2(y1, x1);
            theta2 = Math.Atan2(y2, x2);
            dtheta = theta2 - theta1;
            while (dtheta > Math.PI)
                dtheta -= (Math.PI * 2);
            while (dtheta < -Math.PI)
                dtheta += (Math.PI * 2);
            return (dtheta);
        }        

        /// <summary>
        /// Offset the source polyline to specified side(s).
        /// </summary>
        /// <param name="source">The polyline to be offseted.</param>
        /// <param name="offsetDist">The offset distance.</param>
        /// <param name="side">The offset side(s).</param>
        /// <returns>A polyline sequence resulting from the offset of the source polyline.</returns>
        public static IEnumerable<Polyline> Offset (this Polyline source, double offsetDist, OffsetSide side)
        {
            offsetDist = Math.Abs(offsetDist);
            IEnumerable<Polyline> offsetRight = source.GetOffsetCurves(offsetDist).Cast<Polyline>();
            double areaRight = offsetRight.Select(pline => pline.Area).Sum();
            IEnumerable<Polyline> offsetLeft = source.GetOffsetCurves(-offsetDist).Cast<Polyline>();
            double areaLeft = offsetLeft.Select(pline => pline.Area).Sum();
            switch (side)
            {
                case OffsetSide.In:
                    if (areaRight < areaLeft)
                    {
                        offsetLeft.Dispose();
                        return offsetRight;
                    }
                    else
                    {
                        offsetRight.Dispose();
                        return offsetLeft;
                    }
                case OffsetSide.Out:
                    if (areaRight < areaLeft)
                    {
                        offsetRight.Dispose();
                        return offsetLeft;
                    }
                    else
                    {
                        offsetLeft.Dispose();
                        return offsetRight;
                    }
                case OffsetSide.Left:
                    offsetRight.Dispose();
                    return offsetLeft;
                case OffsetSide.Right:
                    offsetLeft.Dispose();
                    return offsetRight;
                case OffsetSide.Both:
                    return offsetRight.Concat(offsetLeft);
                default:
                    return null;
            }
        }
        public static void Dispose (this IEnumerable<Polyline> plines)
        {
            foreach (Polyline pline in plines)
            {
                pline.Dispose();
            }
        }

        /// <summary>
        /// Проверка наличия самопересечений в полилинии
        /// True - нет самопересечений
        /// </summary>        
        public static bool CheckCross (this Polyline pline)
        {
            MPolygon mpoly = new MPolygon();
            bool isValidBoundary = false;
            try
            {
                mpoly.AppendLoopFromBoundary(pline, true, Tolerance.Global.EqualPoint);
                if (mpoly.NumMPolygonLoops != 0)
                {
                    isValidBoundary = true;
                }
            }
            catch { }
            mpoly.Dispose();
            return isValidBoundary;
        }

        /// <summary>
        /// Следующий индекс полилинии
        /// </summary>
        /// <param name="pl">Полилиния</param>
        /// <param name="index">Текущий индек</param>
        /// <param name="dir">Величина сдвига индекса. 1 - на ед вверх, -1 на ед. вниз, и т.д.</param>
        /// <returns>Текущий индекс + сдвиг. С проверкой попадания индекса в кол-во вершин полилинии.</returns>
        public static int NextVertexIndex (this Polyline pl, int index, int dir =1)
        {
            var res = index+dir;
            if (res < 0)
            {
                res = pl.NumberOfVertices - 1;
            }
            else if (res >= pl.NumberOfVertices)
            {
                res = 0;
            }
            return res;
        }
    }
}
