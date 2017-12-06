using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using JetBrains.Annotations;
using System;
using System.Text;

namespace AcadLib.XData.Viewer
{
    public static class XDataView
    {
        public static void View()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;
            var db = doc.Database;

            var opt = new PromptEntityOptions("\nВыбери приметив:");
            var res = ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                var sbInfo = new StringBuilder();
                var entName = string.Empty;
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

                    if (sbInfo.Length == 0)
                    {
                        ed.WriteMessage("\nНет расширенных данных у {0}", ent);
                        return;
                    }
                    t.Commit();
                }
                var formXdataView = new FormXDataView(sbInfo.ToString(), entName);
                Application.ShowModalDialog(formXdataView);
            }
        }

        [CanBeNull]
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
                    exploreDictionary(item.Value, ref sbInfo, tab + "    ");
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
