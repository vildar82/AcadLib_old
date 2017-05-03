using System.Linq;
using AcadLib.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace TestAcadlib.Comparers
{
    public class TestPoint2dComparer
    {
        [CommandMethod(nameof(TestPoint2dGroup))]
        public void TestPoint2dGroup()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var selRes = ed.GetEntity("Выбери полилинию");
            if (selRes.Status != PromptStatus.OK) return;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var pl = selRes.ObjectId.GetObject(OpenMode.ForWrite) as Polyline;
                if (pl == null) return;

                var pts = pl.GetPoints();                
                var comparer = new AcadLib.Comparers.Point2dEqualityComparer(5);
                var group = pts.GroupBy(g => g, comparer).Select(s=>s.Key).ToList();
                var newPl = group.CreatePolyline();

                var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                cs.AppendEntity(newPl);
                t.AddNewlyCreatedDBObject(newPl, true);
                pl.Erase();

                t.Commit();
            }                   
        }
    }
}
