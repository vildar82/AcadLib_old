using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using AcadLib.Geometry;

namespace AcadLib.Hatches
{
    public static class HatchExt
    {
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
