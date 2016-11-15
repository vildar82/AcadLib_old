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
            using (var entDic = new EntDictExt(dboSave.GetDBObject(), dboSave.PluginName))
            {
                entDic.Save(dboSave.GetExtDic(doc));
            }
        }

        public static void LoadDboDict(this IDboDataSave dboSave)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            using (var entDic = new EntDictExt(dboSave.GetDBObject(), dboSave.PluginName))
            {
                var dicED = entDic.Load();
                dboSave.SetExtDic(dicED, doc);
            }
        }
    }
}
