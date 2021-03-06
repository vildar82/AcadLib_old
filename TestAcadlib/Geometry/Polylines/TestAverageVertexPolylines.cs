﻿using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcadLib.Geometry;
using AcadLib;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace TestAcadlib.Geometry.Polylines
{
    public class TestAverageVertexPolylines
    {
        [CommandMethod(nameof(TestAverageVertexes))]
        public void TestAverageVertexes ()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var selPl1 = ed.GetEntity("Pl1");
            if (selPl1.Status != PromptStatus.OK)
                return;
            var selPl2 = ed.GetEntity("Pl2");
            if (selPl2.Status != PromptStatus.OK)
                return;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var pl1 = selPl1.ObjectId.GetObject(OpenMode.ForWrite) as Polyline;
                var pl2 = selPl2.ObjectId.GetObject(OpenMode.ForWrite) as Polyline;

                pl1.AverageVertexes(ref pl2, new Autodesk.AutoCAD.Geometry.Tolerance(1, 1), true);

                var pls = new List<Polyline> { pl1, pl2 };

                var plUnion = pls.Union(null);

                EntityHelper.AddEntityToCurrentSpace(plUnion);

                t.Commit();
            }
        }
    }
}
