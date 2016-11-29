using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib
{
    public static class EntityHelper
    {
        public static void AddEntityToCurrentSpace (this Entity ent)
        {
            if (ent.IsDisposed) return;
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            using (doc.LockDocument())
            using (var t = db.TransactionManager.StartTransaction())
            {
                var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                if (!ent.IsWriteEnabled) ent.UpgradeOpen();
                cs.AppendEntity(ent);
                db.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(ent, true);
                t.Commit();
            }
        }

        public static DBText CreateText (string text, Point3d pos, EntityOptions entityOptions= null)
        {
            var dbText = new DBText();
            dbText.TextString = text;
            dbText.Position = pos;
            dbText.SetOptions(entityOptions);
            return dbText;
        }
    }
}
