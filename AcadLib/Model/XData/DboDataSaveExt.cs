using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace AcadLib.XData
{
    public static class DboDataSaveExt
    {
        public static void SaveDboDict(this IDboDataSave dboSave)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            using (var dbo = dboSave.GetDBObject())
            {
                using (var entDic = new EntDictExt(dbo, dboSave.PluginName))
                {
                    entDic.Save(dboSave.GetExtDic(doc));
                }
            }
        }

        public static void LoadDboDict(this IDboDataSave dboSave)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            using (var dbo = dboSave.GetDBObject())
            {
                using (var entDic = new EntDictExt(dbo, dboSave.PluginName))
                {
                    var dicED = entDic.Load();
                    dboSave.SetExtDic(dicED, doc);
                }
            }
        }

        /// <summary>
        /// Удаление словаря из объекта
        /// </summary>
        /// <param name="dboSave">Объект чертежа</param>
        /// <param name="dicName">Имя удаляемого словаря или пусто для удаления всего словаря плагина</param>
        public static void DeleteDboDict (this IDboDataSave dboSave, string dicName = null)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            using (var dbo = dboSave.GetDBObject())
            {
                using (var entDic = new EntDictExt(dbo, dboSave.PluginName))
                {
                    entDic.Delete(dicName);
                }
            }
        }
    }
}
