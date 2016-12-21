using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcadLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAcadlib.Geometry.Polylines
{
    public class TestPolyline
    {
        [CommandMethod(nameof(TestPointInsidePolyline))]
        public void TestPointInsidePolyline()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

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
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

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
