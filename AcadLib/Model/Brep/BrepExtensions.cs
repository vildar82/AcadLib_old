using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

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
            List<Region> colReg = new List<Region>();
            foreach (var pl in idsPl)
            {                
                if (pl == null || pl.Area == 0) continue;

                // Создание региона из полилинии
                var dbs = new DBObjectCollection();
                dbs.Add(pl);
                var dbsRegions = Region.CreateFromCurves(dbs);
                if (dbsRegions.Count > 0)
                {
                    Region r = (Region)dbsRegions[0];
                    colReg.Add(r);
                }                
            }

            // Объединение регионов
            Region r1 = colReg.First();
            foreach (var iReg in colReg.Skip(1))
            {
                r1.BooleanOperation(BooleanOperationType.BoolUnite, iReg);
            }            
            return GetRegionContour(r1);            
        }

        private static Polyline3d GetRegionContour(Region reg)
        {
            Polyline3d resVal = null;
            double maxArea = 0;
            Brep brep = new Brep(reg);
            foreach (Autodesk.AutoCAD.BoundaryRepresentation.Face face in brep.Faces)
            {
                foreach (BoundaryLoop loop in face.Loops)
                {
                    if (loop.LoopType == LoopType.LoopExterior)
                    {
                        HashSet<Point3d> ptsHash = new HashSet<Point3d>();                                                
                        foreach (Autodesk.AutoCAD.BoundaryRepresentation.Vertex vert in loop.Vertices)
                        {
                            if (!ptsHash.Any(p => p.IsEqualTo(vert.Point, Tolerance.Global)))
                            {
                                ptsHash.Add(vert.Point);
                            }                            
                        }
                        Point3dCollection pts = new Point3dCollection(ptsHash.ToArray());
                        var pl = new Polyline3d(Poly3dType.SimplePoly, pts, true);
                        if (pl.Area>maxArea)
                        {
                            resVal = pl;
                        }
                    }
                }
            }
            return resVal;
        }
    }    
}
