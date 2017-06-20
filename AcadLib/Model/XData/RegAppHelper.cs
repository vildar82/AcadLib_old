using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    public static class RegAppHelper
    {
        public static void RegApp(this Database db, string regAppName)
        {
            using (var rat = db.RegAppTableId.Open(OpenMode.ForRead, false) as RegAppTable)
            {
                if (rat.Has(regAppName)) return;
                rat.UpgradeOpen();
                using (var ratr = new RegAppTableRecord())
                {
                    ratr.Name = regAppName;
                    rat.Add(ratr);
                }
            }
        }

        public static void RegAppPIK(this Database db)
        {
            RegAppHelper.RegApp(db,General.Company);
        }
    }
}
