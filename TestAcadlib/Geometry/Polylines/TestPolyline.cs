using System.Collections.Generic;
using AcadLib;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcadLib.Geometry;
using Autodesk.AutoCAD.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly:CommandClass(typeof(TestAcadlib.Geometry.Polylines.TestPolyline))]

namespace TestAcadlib.Geometry.Polylines
{
    public class TestPolyline
    {
        [CommandMethod(nameof(TestTwoPolylineIntersection))]
        public void TestTwoPolylineIntersection()
        {
            CommandStart.Start(doc =>
            {
                var db = doc.Database;
                var ed = doc.Editor;
                using (var t = db.TransactionManager.StartTransaction())
                {
                    var pl1 = ed.SelectEntity<Polyline>("Выбери полилинию 1 для проверки самопересечения", "Полилинию")
                        .GetObject<Polyline>();
                    var pl2 = ed.SelectEntity<Polyline>("Выбери полилинию 2 для проверки самопересечения", "Полилинию")
                        .GetObject<Polyline>();
                    var intersections = pl1.Intersects(pl2);
                    foreach (var intersect in intersections)
                    {
                        Inspector.AddError($"Самопересечение в точке {intersect}", intersect.GetRectangleFromCenter(2), pl1.Id);
                    }
                    t.Commit();
                }
            });
        }

        [CommandMethod(nameof(TestPolylineSelfIntersection))]
        public void TestPolylineSelfIntersection()
        {
            CommandStart.Start(doc =>
            {
                var db = doc.Database;
                var ed = doc.Editor;
                using (var t = db.TransactionManager.StartTransaction())
                {
                    var plId = ed.SelectEntity<Polyline>("Выбери полилинию для проверки самопересечения", "Полилинию");
                    var pl = plId.GetObject<Polyline>();
                    var validRes = pl.IsValidPolygon(out List<Point3d> intersections);
                    validRes.ToString().WriteToCommandLine();
                    foreach (var intersect in intersections)
                    {
                        Inspector.AddError($"Самопересечение в точке {intersect}", intersect.GetRectangleFromCenter(2),plId);
                    }
                    t.Commit();
                }
            });
        }

        [CommandMethod(nameof(TestPointInsidePolyline))]
        public void TestPointInsidePolyline()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var selOpt = new PromptEntityOptions("\nВыбери полилинию");
            selOpt.SetRejectMessage("\nТолько полилинию");
            selOpt.AddAllowedClass(typeof(Polyline), true);
            var selRes = ed.GetEntity(selOpt);
            if (selRes.Status != PromptStatus.OK) return;

            var pt = ed.GetPointWCS("\nУкажи точку");

            using (var t = db.TransactionManager.StartTransaction())
            {
                var pl = selRes.ObjectId.GetObject( OpenMode.ForRead) as Polyline;
                var res = pl.IsPointInsidePolygon(pt) ? "Внутри": "Снаружи";
                ed.WriteMessage($"\n{res}");
                t.Commit();
            }
        }


        [CommandMethod(nameof(TestPointOnPolyline))]
        public void TestPointOnPolyline()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var selOpt = new PromptEntityOptions("\nВыбери полилинию");
            selOpt.SetRejectMessage("\nТолько полилинию");
            selOpt.AddAllowedClass(typeof(Polyline), true);
            var selRes = ed.GetEntity(selOpt);
            if (selRes.Status != PromptStatus.OK) return;

            var pt = ed.GetPointWCS("\nУкажи точку");

            using (var t = db.TransactionManager.StartTransaction())
            {
                var pl = selRes.ObjectId.GetObject(OpenMode.ForRead) as Polyline;
                var res = pl.IsPointOnPolyline(pt, 1) ? "Да" : "Нет";
                ed.WriteMessage($"\n{res}");
                t.Commit();
            }
        }
    }
}
