using System;
using System.Linq;
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
            if (dbo.ExtensionDictionary.IsNull) return;
            var extDic = dbo.ExtensionDictionary.GetObject(OpenMode.ForWrite) as DBDictionary;
            if (extDic == null) return;
            dbo.UpgradeOpen();
            foreach (var entry in extDic)
            {
                extDic.Remove(entry.Key);
            }
            dbo.ReleaseExtensionDictionary();
        }

        /// <summary>
        /// Удаление расширенных данных из объекта
        /// </summary>
        /// <param name="dbo"></param>
        public static void RemoveAllXData (this DBObject dbo)
        {
            if (dbo.XData == null) return;
            var appNames = from TypedValue tv in dbo.XData.AsArray() where tv.TypeCode == 1001 select tv.Value.ToString();
            dbo.UpgradeOpen();
            foreach (var appName in appNames)
            {
                dbo.XData = new ResultBuffer(new TypedValue(1001, appName));
            }
        }        
    }
}
