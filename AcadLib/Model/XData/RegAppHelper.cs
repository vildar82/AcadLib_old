using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;

namespace AcadLib
{
    public static class RegAppHelper
    {
        public static void RegApp([NotNull] this Database db, string regAppName)
        {
#pragma warning disable 618
            using (var rat = (RegAppTable)db.RegAppTableId.Open(OpenMode.ForRead, false))
#pragma warning restore 618
            {
                if (rat.Has(regAppName)) return;
                // ReSharper disable once UpgradeOpen
                rat.UpgradeOpen();
                using (var ratr = new RegAppTableRecord())
                {
                    ratr.Name = regAppName;
                    rat.Add(ratr);
                }
            }
        }

        public static void RegAppPIK([NotNull] this Database db)
        {
            RegApp(db, General.Company);
        }
    }
}
