using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcadLib.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace TestAcadlib.Geometry.Polylines
{
    public class TestPolyline
    {
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
