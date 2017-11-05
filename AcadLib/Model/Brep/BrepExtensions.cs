using System;
using System.Collections.Generic;
using System.Linq;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using AcadLib.Geometry;
using AcadLib.Hatches;
using Extensions;
using NetLib;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace AcadLib
{
    public static class BrepExtensions
    {
        /// <summary>
        /// Определение контура для набора полилиний - объекдинением в регион и извлечением внешнего его контура.
        /// Должна быть запущена транзакция
        /// </summary>        
        public static Polyline3d GetExteriorContour(this List<Polyline> idsPl)
        {
            var colReg = new List<Region>();
            foreach (var pl in idsPl)
            {                
                if (pl == null || Math.Abs(pl.Area) < 0.0001) continue;

                // Создание региона из полилинии
	            var dbs = new DBObjectCollection {pl};
	            var dbsRegions = Region.CreateFromCurves(dbs);
                if (dbsRegions.Count > 0)
                {
                    var r = (Region)dbsRegions[0];
                    colReg.Add(r);
                    foreach (var item in dbsRegions.Cast<DBObject>().Skip(1))
                    {
                        item.Dispose();
                    }
                }                
            }

            // Объединение регионов
            var r1 = colReg.First();            
            foreach (var iReg in colReg.Skip(1))
            {
                r1.BooleanOperation(BooleanOperationType.BoolUnite, iReg);
            }
            foreach (var item in colReg.Skip(1))
            {
                item.Dispose();
            }
            return GetRegionContour(r1);            
        }

        public static Polyline3d GetRegionContour(this Region reg)
        {
            Polyline3d resVal = null;
            double maxArea = 0;
            var brep = new Brep(reg);
            foreach (var face in brep.Faces)
            {
                foreach (var loop in face.Loops)
                {
                    if (loop.LoopType == LoopType.LoopExterior)
                    {
                        var ptsVertex = new List<Point3d>();                                                
                        foreach (var vert in loop.Vertices)
                        {
                            if (!ptsVertex.Any(p => p.IsEqualTo(vert.Point, Tolerance.Global)))
                            {
                                ptsVertex.Add(vert.Point);
                            }                            
                        }
                        var pts = new Point3dCollection(ptsVertex.ToArray());
                        var pl = new Polyline3d(Poly3dType.SimplePoly, pts, true);
	                    var plArea = pl.Area;
						if (plArea>maxArea)
                        {
                            resVal = pl;
	                        maxArea = plArea;
                        }
                    }
                }
            }
            return resVal;
        }

        /// <summary>
        /// Без дуговых сегментов!!!
        /// </summary>
        public static List<KeyValuePair<Polyline, BrepLoopType>> GetPolylines (this Region reg)
        {            
            var resVal = new List<KeyValuePair<Polyline, BrepLoopType>>(); ;
            var brep = new Brep(reg);
            foreach (var face in brep.Faces)
            {
                foreach (var loop in face.Loops)
                {
                    var ptsVertex = new List<Point2d>();
                    foreach (var vert in loop.Vertices)                    
                        ptsVertex.Add(vert.Point.Convert2d());

                    var pl = ptsVertex.CreatePolyline();
                    resVal.Add(new KeyValuePair<Polyline, BrepLoopType>(pl, (BrepLoopType)loop.LoopType));
                }
            }
            return resVal;
        }

        public static List<KeyValuePair<Point2dCollection, BrepLoopType>> GetPoints2dByLoopType (this Region reg)
        {            
            var resVal = new List<KeyValuePair<Point2dCollection, BrepLoopType>>();
            var brep = new Brep(reg);
            foreach (var face in brep.Faces)
            {
                foreach (var loop in face.Loops)
                {
                    var pts2dCol = new Point2dCollection(loop.Vertices.Select(vert => vert.Point.Convert2d()).ToArray());
                    resVal.Add(new KeyValuePair<Point2dCollection, BrepLoopType>(pts2dCol, (BrepLoopType)loop.LoopType));
                }
            }
            return resVal;
        }

        public static List<Point3d> GetVertices (this Region reg)
        {
            var ptsVertex = new List<Point3d>();
            var brep = new Brep(reg);
            foreach (var face in brep.Faces)
            {
                foreach (var loop in face.Loops)
                {                    
                    foreach (var vert in loop.Vertices)
                    {
                        ptsVertex.Add(vert.Point);
                    }
                }
            }
            return ptsVertex;
        }

        public static Hatch CreateHatch(this Region region, bool createOut, out DisposableSet<Polyline> externalLoops)
        {
            externalLoops = createOut ? new DisposableSet<Polyline>() : null;
            var plsByLoop = region.GetPoints2dByLoopType();
            var extLoops = plsByLoop.Where(p => p.Value != BrepLoopType.LoopInterior).ToList();
            var intLoops = plsByLoop.Where(p => p.Value == BrepLoopType.LoopInterior).ToList();

            if (!extLoops.Any())
            {
                return null;
            }

            var h = new Hatch();
            h.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");

            foreach (var item in extLoops)
            {
                var pts2dCol = item.Key;
                pts2dCol.Add(item.Key[0]);
                h.AppendLoop(HatchLoopTypes.External, pts2dCol, new DoubleCollection(new double[extLoops.Count + 1]));
                if (createOut)
                {
                    externalLoops.Add(pts2dCol.Cast<Point2d>().ToList().CreatePolyline());
                }
            }

            if (intLoops.Any())
            {
                foreach (var item in intLoops)
                {
                    var pts2dCol = item.Key;
                    pts2dCol.Add(item.Key[0]);
                    h.AppendLoop(HatchLoopTypes.SelfIntersecting, pts2dCol, new DoubleCollection(new double[intLoops.Count + 1]));
                }
            }

            h.EvaluateHatch(true);
            return h;
        }
        public static Hatch CreateHatch (this Region region)
        {
            return CreateHatch(region, false, out _);
        }

        public static Region Union(this List<Polyline> pls, Region over)
        {
            return Union((IEnumerable<Polyline>)pls, over);
        }

        /// <summary>
        /// Объекдинение полилиний.
        /// Полилинии должны быть замкнуты!
        /// </summary>        
        /// <param name="over">Контур который должен быть "над" объединенными полилиниями. Т.е. контур этой полилинии вырезается из полученного контура, если попадает на него.</param>
        public static Region Union (this IEnumerable<Polyline> pls, Region over)
        {
            if (pls == null || !pls.Any()) return null;            
            var regions = CreateRegion(pls);
            Region union = null;
            try
            {
                union = UnionRegions(regions);
            }
            finally
            {
                regions.Remove(union);
                foreach (var item in regions)
                {
                    item.Dispose();
                }
            }
            
            // Вырезание over региона
            if (over != null)
            {
                union.BooleanOperation(BooleanOperationType.BoolSubtract, over);
            }                                          
            return union;
        }

	    public static Region CreateRegion(this Polyline pl)
	    {
	        return CreateRegion((Curve) pl);
	    }

        public static Region CreateRegion(this Curve curve)
        {
            if (curve == null) return null;
            var dbs = new DBObjectCollection { curve };
            var dbsRegs = Region.CreateFromCurves(dbs);
            if (dbsRegs == null || dbsRegs.Count == 0) return null;
            if (dbsRegs.Count == 1) return (Region)dbsRegs[0];
            var reg = (Region)dbsRegs[0];
            foreach (var obj in dbsRegs.Cast<Region>().Skip(1))
            {
                obj.Dispose();
            }
            return reg;
        }

        public static Region CreateRegion(this Hatch hatch)
        {
            try
            {
                if (hatch == null) return null;
                using (var loops = hatch.GetPolylines2(Block.Tolerance01,
                    HatchLoopTypes.Polyline | HatchLoopTypes.Default | HatchLoopTypes.Derived
                    | HatchLoopTypes.External | HatchLoopTypes.Outermost | HatchLoopTypes.NotClosed |
                    HatchLoopTypes.SelfIntersecting, false))
                {
                    var validLoops = loops.Where(w => w.Loop.Area > 0).ToList();
                    var externalLoops = new List<Curve>();
                    var internalLoop = new List<Curve>();
                    foreach (var loop in validLoops)
                    {
                        if (loop.Types.Has(HatchLoopTypes.External))
                            externalLoops.Add(loop.Loop);
                        else if (loop.Types.HasAny(HatchLoopTypes.Derived | HatchLoopTypes.Outermost))
                            internalLoop.Add(loop.Loop);
                        else
                        {
                            Inspector.AddError($"Тип контура {loop.Types} в штриховке пропущен", hatch);
                        }
                    }
                    if (!externalLoops.Any())
                    {
                        Inspector.AddError($"Штриховка без внешних контуров - пропущена", hatch);
                    }
                    var externalRegion = GetRegion(externalLoops);
                    if (internalLoop.Any())
                    {
                        var internalRegion = GetRegion(internalLoop);
                        externalRegion.BooleanOperation(BooleanOperationType.BoolSubtract, internalRegion);
                        internalRegion.Dispose();
                    }
                    var region = externalRegion;
                    return region;
                }
            }
            catch (Exception ex)
            {
                Inspector.AddError($"ошибка определения области штриховки. Пропущена. {ex.Message}.", hatch);
                return null;
            }
        }
        private static Region GetRegion(IEnumerable<Curve> pls)
        {
            using (var regions = new DisposableSet<Region>(pls.CreateRegion()))
            {
                var reg = regions.Skip(1).Any() ? regions.ToList().UnionRegions() : regions.First();
                regions.Remove(reg);
                return reg;
            }
        }

        public static List<Region> CreateRegion (this IEnumerable<Polyline> pls)
		{
			return CreateRegion(pls.Cast<Curve>());
        }

	    public static List<Region> CreateRegion(this IEnumerable<Curve> curves)
	    {
		    var res = new List<Region>();
		    var dbs = new DBObjectCollection();
		    foreach (var curve in curves)
		    {
			    dbs.Add(curve);
		    }
		    var dbsRegions = Region.CreateFromCurves(dbs);
		    foreach (var item in dbsRegions)
		    {
			    res.Add((Region)item);
		    }
#if DEBUG
//EntityHelper.AddEntityToCurrentSpace(res);
#endif
		    return res;
	    }

		public static Region UnionRegions (this List<Region> regions)
        {
            if (regions?.Any() != true) return null;
            if (regions.Count ==1) return regions[0];           
            var union = regions.First();            
            for (var i = 1; i < regions.Count; i++)
            {
                var cr = regions[i];
                union.BooleanOperation(BooleanOperationType.BoolUnite, cr);                
            }
            return union;
        }
    }    
}
