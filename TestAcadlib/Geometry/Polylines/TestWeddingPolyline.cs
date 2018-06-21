using AcadLib.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace TestAcadlib.Geometry.Polylines
{
    public class TestWeddingPolyline
    {
        [CommandMethod(nameof(TestWEdding))]
        public void TestWEdding()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var selOpt = new PromptEntityOptions("\nВыбери полилинию для прополки");
            selOpt.SetRejectMessage("\nТолько полилинию");
            selOpt.AddAllowedClass(typeof(Polyline), true);
            var selRes = ed.GetEntity(selOpt);
            if (selRes.Status != PromptStatus.OK)
                return;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var pl = selRes.ObjectId.GetObject(OpenMode.ForWrite) as Polyline;
                pl.TestDrawVertexNumbers(Color.FromColor(System.Drawing.Color.Green));

                pl.Wedding(new Autodesk.AutoCAD.Geometry.Tolerance (0.02,0.1));

                pl.TestDrawVertexNumbers(Color.FromColor(System.Drawing.Color.Red));
                t.Commit();
            }
        }
    }
}
