namespace TestAcadlib.Brep
{
    using System.Collections.Generic;
    using AcadLib;
    using Autodesk.AutoCAD.ApplicationServices.Core;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Runtime;

    public class TestBrep
    {
        [CommandMethod("TestBrepCreateRegionHatch")]
        public void TestBrepCreateRegionHatch()
        {
            CommandStart.Start(doc =>
            {
                var ed = doc.Editor;
                using (var t = doc.TransactionManager.StartTransaction())
                {
                    var h = ed.SelectEntity<Hatch>("Выбери штриховку для построения региона", "Штриховку")
                        .GetObjectT<Hatch>();
                    var region = h.CreateRegion();
                    if (region == null)
                        throw new System.Exception($"Пустой регион = null.");
                    region.AddEntityToCurrentSpace();
                    t.Commit();
                }
            });
        }

        [CommandMethod("TestBrepUnion")]
        public void TestBrepUnion()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var tvs = new[] { new TypedValue((int)DxfCode.Start, "LWPOLYLINE") };
            var selFilter = new SelectionFilter(tvs);
            var sel = ed.GetSelection(selFilter);
            if (sel.Status != PromptStatus.OK)
                return;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var idsPls = sel.Value.GetObjectIds();
                var pls = new List<Polyline>();
                foreach (var item in idsPls)
                {
                    var pl = item.GetObject(OpenMode.ForRead) as Polyline;
                    pls.Add(pl);
                }

                var union = pls.Union(null);

                t.Commit();
            }
        }
    }
}