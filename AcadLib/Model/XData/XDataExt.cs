using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    public static class XDataExt
    {
        public const string PikApp = "PIK";
        /// <summary>
        /// Регистрация приложения в RegAppTable
        /// </summary>        
        public static void RegApp(this Database db, string regAppName)
        {
            using (RegAppTable rat = db.RegAppTableId.Open(OpenMode.ForRead, false) as RegAppTable)
            {
                if (!rat.Has(regAppName))
                {
                    rat.UpgradeOpen();
                    using (RegAppTableRecord ratr = new RegAppTableRecord())
                    {
                        ratr.Name = regAppName;
                        rat.Add(ratr);
                    }
                }
            }
        }

        /// <summary>
        /// Регистрация приложения PIK в RegAppTable
        /// </summary>
        /// <param name="db"></param>
        public static void RegAppPIK(this Database db)
        {
            RegApp(db, PikApp);
        }

        /// <summary>
        /// Удаление XData связанных с этим приложением
        /// </summary>
        /// <param name="dbo"></param>
        /// <param name="regAppName"></param>
        public static void RemoveXData(this DBObject dbo, string regAppName)
        {
            if (dbo.GetXDataForApplication(regAppName) != null)
            {
                ResultBuffer rb = rb = new ResultBuffer(new TypedValue(1001, regAppName));
                dbo.UpgradeOpen();
                dbo.XData = rb;
                dbo.DowngradeOpen();
            }
        }

        public static void RemoveXDataPIK(this DBObject dbo)
        {
            RemoveXData(dbo, PikApp);
        }

        public static void SetXData(this DBObject dbo, string regAppName, int value)
        {
            using (var rb = new ResultBuffer(
                        new TypedValue((int)DxfCode.ExtendedDataRegAppName, regAppName),
                        new TypedValue((int)DxfCode.ExtendedDataInteger32, value)))
            {
                dbo.XData = rb;
            }
        }

        public static void SetXDataPIK(this DBObject dbo, int value)
        {
            SetXData(dbo, PikApp, value);
        }
        
        public static int GetXData(this DBObject dbo, string regAppName)
        {
            var rb = dbo.GetXDataForApplication(regAppName);
            if (rb != null)
            {
                foreach (var item in rb)
                {
                    if (item.TypeCode == (short)DxfCode.ExtendedDataInteger32)
                    {
                        return (int)item.Value;
                    }
                }
            }
            return 0;
        }
        public static int GetXDatPIK(this DBObject dbo)
        {
            return GetXData(dbo, PikApp);
        }

        public static string GetXDataString(this DBObject dbo, string regAppName)
        {
            var rb = dbo.GetXDataForApplication(regAppName);
            if (rb != null)
            {
                foreach (var item in rb)
                {
                    if (item.TypeCode == (short)DxfCode.ExtendedDataAsciiString)
                    {
                        return (string)item.Value;
                    }
                }
            }
            return string.Empty;
        }
        public static string GetXDatPIKString(this DBObject dbo)
        {
            return GetXDataString(dbo, PikApp);
        }
    }
}
