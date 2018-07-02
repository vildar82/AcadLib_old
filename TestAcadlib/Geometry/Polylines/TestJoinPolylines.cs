using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Geometry;
using AcadLib.Geometry.Polylines.Join;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace TestAcadlib.Geometry.Polylines
{
    public class TestJoinPolylines
    {
        [CommandMethod(nameof(TestJoinPolylinesCommand1))]
        public void TestJoinPolylinesCommand1()
        {
            var doc = AcadHelper.Doc;
            var db = doc.Database;
            var ed = doc.Editor;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject<BlockTableRecord>();
                foreach (var polylines in ms.GetObjects<Polyline>(OpenMode.ForWrite).GroupBy(g => g.Color))
                {
                    var pls = polylines.ToList();
                    var joinedPls = new List<Polyline>();
                    pls.ToList().Join(ref joinedPls, disposePls: false, wedding: true,
                        tolerance: new Tolerance(0.005, 0.005));
                    foreach (var polyline in pls.Except(joinedPls))
                    {
                        polyline.Erase();
                    }
                }

                t.Commit();
            }
        }
    }
}
