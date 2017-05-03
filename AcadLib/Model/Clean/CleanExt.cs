using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    public static class CleanExt
    {
        public static int CleanZombieBlock(this Database db)
        {
            var countZombie = 0;
            using (var t = db.TransactionManager.StartTransaction())
            {
                var bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                foreach (var idBtr in bt)
                {
                    var btr = idBtr.GetObject(OpenMode.ForRead) as BlockTableRecord;
                    if (!btr.IsLayout && btr.IsAnonymous && !btr.IsDynamicBlock && btr.Name.StartsWith("*U"))
                    {
                        var idBlRefs = btr.GetBlockReferenceIds(true, false);
                        if (idBlRefs.Count == 0) continue;
                        var isZombie = true;
                        foreach (ObjectId idBlRef in idBlRefs)
                        {
                            var blRef = idBlRef.GetObject(OpenMode.ForWrite, false, true) as BlockReference;
                            if (!blRef.AnonymousBlockTableRecord.IsNull)
                            {
                                isZombie = false;
                                break;                                
                            }                            
                            blRef.Erase();
                            countZombie++;
                        }
                        if (isZombie)
                        {
                            btr.UpgradeOpen();
                            btr.Erase();
                        }
                    }
                }
                t.Commit();
            }
            return countZombie;
        }
    }
}
