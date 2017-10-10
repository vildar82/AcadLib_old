using System;
using System.Collections.Generic;
using System.Linq;
using AcadLib.Errors;
using Autodesk.AutoCAD.Geometry;
using AcadLib.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using NetLib;

namespace AcadLib.Hatches
{
    public static class HatchExt
    {
        public static void SetHatchOptions(this Hatch h, HatchOptions opt)
        {
            if (h == null || opt == null) return;
            if (opt.PatternAngle != null && opt.PatternAngle.Value >0)
            {
                h.PatternAngle = opt.PatternScale.Value;
            }
            if (opt.PatternScale != null && opt.PatternScale.Value >0)
            {
                h.PatternScale = opt.PatternScale.Value;
            }
            if (!opt.PatternName.IsNullOrEmpty())
            {
                h.SetHatchPattern(opt.PatternType, opt.PatternName);
            }
        }

	    public static DisposableSet<HatchLoopPl> GetPolylines2(this Hatch ht, Tolerance weddingTolerance,
			HatchLoopTypes loopType = HatchLoopTypes.External, bool wedding = false)
	    {
			var loops = new DisposableSet<HatchLoopPl>();
		    var nloops = ht.NumberOfLoops;
		    for (var i = 0; i < nloops; i++)
		    {
			    var loop = ht.GetLoopAt(i);
			    if (loopType.HasAny(loop.LoopType))
			    {
				    var poly = new Polyline();
				    var vertex = 0;
				    if (loop.IsPolyline)
				    {
					    foreach (BulgeVertex bv in loop.Polyline)
					    {
						    poly.AddVertexAt(vertex++, bv.Vertex, bv.Bulge, 0.0, 0.0);
					    }
				    }
				    else
				    {
					    foreach (Curve2d curve in loop.Curves)
					    {
						    if (curve is LinearEntity2d l)
						    {
							    if (NeedAddVertexToPl(poly, vertex-1, l.StartPoint, weddingTolerance))
							    {
									poly.AddVertexAt(vertex++, l.StartPoint, 0, 0, 0);
								}
							    poly.AddVertexAt(vertex++, l.EndPoint, 0, 0, 0);
						    }
						    else if (curve is CircularArc2d arc)
						    {
							    if (arc.IsCircle())
							    {
								    loops.Add(new HatchLoopPl {Loop = arc.CreateCircle(), Types = loop.LoopType});
									continue;
							    }
							    var bulge = arc.GetBulge(arc.IsClockWise);
								if (NeedAddVertexToPl(poly, vertex-1, arc.StartPoint, weddingTolerance))
							    {
								    poly.AddVertexAt(vertex++, arc.StartPoint, bulge, 0, 0);
							    }
							    else
							    {
								    poly.SetBulgeAt(vertex-1, bulge);
							    }
							    poly.AddVertexAt(vertex++, arc.EndPoint, 0, 0, 0);
						    }
						    else
						    {
							    Inspector.AddError($"Тип сегмента штриховки не поддерживается {curve}", ht);
						    }
					    }
				    }
				    if (poly.NumberOfVertices != 0)
				    {
					    if (wedding)
					    {
						    poly.Wedding(weddingTolerance);
					    }
						if (!poly.Closed) poly.Closed = true;
					    loops.Add(new HatchLoopPl {Loop = poly, Types = loop.LoopType});
				    }
			    }
		    }
		    return loops;
		}

	    private static bool NeedAddVertexToPl(Polyline poly, int prewVertex, Point2d vertex, Tolerance tolerance)
	    {
		    return prewVertex <= 0 || !poly.GetPoint2dAt(prewVertex - 1).IsEqualTo(vertex, tolerance);
	    }

		/// <summary>
		/// Полилинии в штриховке
		/// </summary>
		/// <param name="ht">Штриховка</param>
		/// <param name="loopType">Из каких типов островков</param>    
		public static DisposableSet<Polyline> GetPolylines(this Hatch ht, HatchLoopTypes loopType = HatchLoopTypes.External)
		{
			var loops = GetPolylines2(ht, Tolerance.Global,loopType);
			var res = new DisposableSet<Polyline>(loops.Select(s => s.GetPolyline()));
			loops.Clear();
			return res;
        }    

        /// <summary>
        /// Создание ассоциативной штриховки по полилинии
        /// Полилиния должна быть в базе чертежа
        /// </summary>        
        public static Hatch CreateAssociativeHatch (Curve loop, BlockTableRecord cs, Transaction t,
            string pattern = "SOLID", string layer = null, LineWeight lw = LineWeight.LineWeight015)
        {
            var h = new Hatch();
            h.SetDatabaseDefaults();
            if (layer != null)
            {
                Layers.LayerExt.CheckLayerState(layer);
                h.Layer = layer;
            }
            h.LineWeight = lw;
            h.Linetype = SymbolUtilityServices.LinetypeContinuousName;            
            h.SetHatchPattern(HatchPatternType.PreDefined, pattern);
            cs.AppendEntity(h);
            t.AddNewlyCreatedDBObject(h, true);
            h.Associative = true;
            h.HatchStyle = HatchStyle.Normal;

            // добавление контура полилинии в гштриховку
            var ids = new ObjectIdCollection();
            ids.Add(loop.Id);
            try
            {
                h.AppendLoop(HatchLoopTypes.Default, ids);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"CreateAssociativeHatch");
                h.Erase();
                return null;
            }
            h.EvaluateHatch(true);

            var orders = cs.DrawOrderTableId.GetObject(OpenMode.ForWrite) as DrawOrderTable;
            orders.MoveToBottom(new ObjectIdCollection(new[] { h.Id }));            

            return h;
        }

        public static Hatch CreateHatch (this List<Point2d> pts)
        {
            pts = pts.DistinctPoints();            
            var ptCol = new Point2dCollection(pts.ToArray());
            ptCol.Add(pts[0]);
            var dCol = new DoubleCollection(new double[pts.Count]);
            var h = new Hatch();
            h.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");            
            h.AppendLoop(HatchLoopTypes.Default, ptCol, dCol);
            h.EvaluateHatch(false);
            return h;
        }

        public static Hatch CreateHatch (this List<Point3d> pts)
        {
            return CreateHatch(pts.ConvertAll(Point3dExtensions.Convert2d));
        }

        public static Hatch CreateHatch (this Polyline pl)
        {
            var pts = pl.GetPoints();
            var h = CreateHatch(pts);
            return h;
        }
    }
}
