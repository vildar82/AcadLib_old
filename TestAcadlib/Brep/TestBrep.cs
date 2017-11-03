using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcadLib;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace TestAcadlib.Brep
{
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
                        .GetObject<Hatch>();
                    var region = h.CreateRegion();
                    if (region == null) throw new System.Exception($"Пустой регион = null.");
                    region.AddEntityToCurrentSpace();
                    t.Commit();
                }
            });
        }

        [CommandMethod("TestBrepUnion")]
        public void TestBrepUnion ()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var tvs = new TypedValue[] { new TypedValue((int)DxfCode.Start, "LWPOLYLINE") };
            var selFilter = new SelectionFilter(tvs);                                        
            var sel = ed.GetSelection(selFilter);
            if (sel.Status != PromptStatus.OK) return;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var idsPls = sel.Value.GetObjectIds();
                var pls = new List<Polyline>();
                foreach (var item in idsPls)
                {
                    var pl = item.GetObject(OpenMode.ForRead) as Polyline;                    
                    pls.Add(pl);
                }

                var union = BrepExtensions.Union(pls, null);               

                //var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;                
                //if (union != null)
                //{                     
                //    cs.AppendEntity(union);
                //    t.AddNewlyCreatedDBObject(union, true);
                //}                

                t.Commit();
            }
        }
    }
}
