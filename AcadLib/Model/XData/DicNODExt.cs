using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;

namespace AcadLib.XData
{
    public static class DicNODExt
    {
        [CanBeNull]
        public static DicED LoadDicNOD(this Database db, string dicName)
        {
            var nod = new DictNOD(dicName, db);
            return nod.LoadED();
        }

        public static void SaveDicNOD(this Database db, [NotNull] DicED dic)
        {
            var nod = new DictNOD(dic.Name, db);
            nod.Save(dic);
        }
    }
}
