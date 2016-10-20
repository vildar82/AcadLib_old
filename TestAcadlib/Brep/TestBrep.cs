using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcadLib;

namespace TestAcadlib.Brep
{
    public class TestBrep
    {
        [CommandMethod("TestBrepUnion")]
        public void TestBrepUnion ()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            var tvs = new TypedValue[] { new TypedValue((int)DxfCode.Start, "LWPOLYLINE") };
            var selFilter = new SelectionFilter(tvs);                                        
            var sel = ed.GetSelection(selFilter);
            if (sel.Status != PromptStatus.OK) return;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var idsPls = sel.Value.GetObjectIds();
                List<Polyline> pls = new List<Polyline>();
                foreach (var item in idsPls)
                {
                    var pl = item.GetObject(OpenMode.ForRead) as Polyline;                    
                    pls.Add(pl);
                }

                Region union = BrepExtensions.Union(pls, null);               

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
