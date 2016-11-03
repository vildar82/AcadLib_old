using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace TestAcadlib.Geometry.Polylines
{
    public class TestMergePolyline
    {
        [CommandMethod(nameof(TestMerge))]
        public void TestMerge()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            var selRes = ed.Select("Выбери полилинии для объединения");
            if (selRes.Count==0) return;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var pls = new List<Polyline>();
                foreach (var item in selRes)
                {
                    if (!item.IsValidEx()) continue;
                    var pl = item.GetObject(OpenMode.ForRead) as Polyline;
                    if (pl != null)
                    {
                        pls.Add(pl);
                    }                            
                }

                try
                {
                    var plMerged = pls.Merge();

                    var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                    cs.AppendEntity(plMerged);
                    t.AddNewlyCreatedDBObject(plMerged, true);
                    plMerged.ColorIndex = 5;
                }
                catch(System.Exception ex)
                {
                    Application.ShowAlertDialog(ex.ToString());
                }

                t.Commit();
            }
        }            
    }
}
