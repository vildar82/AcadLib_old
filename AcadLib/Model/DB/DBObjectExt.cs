using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    public static class DBObjectExt
    {
        /// <summary>
        /// Удаление словаря из объекта.
        /// </summary>
        /// <param name="dbo"></param>
        public static void RemoveAllExtensionDictionary (this DBObject dbo)
        {
            if (!dbo.ExtensionDictionary.IsNull)
            {
                DBDictionary extDic = dbo.ExtensionDictionary.GetObject(OpenMode.ForWrite) as DBDictionary;
                dbo.UpgradeOpen();
                foreach (DBDictionaryEntry entry in extDic)
                {
                    extDic.Remove(entry.Key);
                }
                dbo.ReleaseExtensionDictionary();
            }
        }

        /// <summary>
        /// Удаление расширенных данных из объекта
        /// </summary>
        /// <param name="dbo"></param>
        public static void RemoveAllXData (this DBObject dbo)
        {
            if (dbo.XData != null)
            {
                IEnumerable<string> appNames = from TypedValue tv in dbo.XData.AsArray() where tv.TypeCode == 1001 select tv.Value.ToString();
                dbo.UpgradeOpen();
                foreach (string appName in appNames)
                {
                    dbo.XData = new ResultBuffer(new TypedValue(1001, appName));
                }
            }
        }        
    }
}
