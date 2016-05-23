using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace AcadLib.XData.Viewer
{
    public static class XDataView 
    {
        public static void View()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            var opt = new PromptEntityOptions("\nВыбери приметив:");
            var res = ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                StringBuilder sbInfo = new StringBuilder();
                string entName = string.Empty;
                using (var t = db.TransactionManager.StartTransaction())
                {                    
                    var ent = res.ObjectId.GetObject(OpenMode.ForRead, false, true) as Entity;
                    entName = ent.ToString();
                    if (ent.XData != null)                    
                    {
                        sbInfo.AppendLine("XData:");
                        foreach (var item in ent.XData)
                        {
                            sbInfo.AppendLine($"    {getTypeCodeName(item.TypeCode)} = {item.Value}");
                        }
                    }
                    if (!ent.ExtensionDictionary.IsNull)
                    {
                        sbInfo.AppendLine("\nExtensionDictionary:");                        
                        exploreDictionary(ent.ExtensionDictionary, ref sbInfo);
                    }

                    if (sbInfo.Length==0)
                    {
                        ed.WriteMessage("\nНет расширенных данных у {0}", ent);
                        return;
                    }
                    t.Commit();
                }                
                FormXDataView formXdataView = new FormXDataView(sbInfo.ToString(), entName);
                Application.ShowModalDialog(formXdataView);
            }
        }

        private static string getTypeCodeName(short typeCode)
        {
            return Enum.GetName(typeof(DxfCode), typeCode);
        }

        private static void exploreDictionary(ObjectId idDict, ref StringBuilder sbInfo, string tab = "    ")
        {
            var entry = idDict.GetObject(OpenMode.ForRead);
            if (entry is DBDictionary)
            {
                var dict = entry as DBDictionary;
                foreach (var item in dict)
                {
                    sbInfo.AppendLine($"{tab}{item.Key}");           
                    exploreDictionary(item.Value, ref sbInfo, tab+"    ");
                }
            }
            else if (entry is Xrecord)
            {
                var xrec = entry as Xrecord;
                foreach (var item in xrec)
                {
                    sbInfo.AppendLine($"{tab}    {getTypeCodeName(item.TypeCode)} = {item.Value}");
                }
            }            
        }
    }
}
