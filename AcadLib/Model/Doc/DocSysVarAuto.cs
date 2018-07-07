namespace AcadLib.Doc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using AutoCAD_PIK_Manager.Settings;
    using Autodesk.AutoCAD.ApplicationServices;
    using DynamicData;
    using JetBrains.Annotations;
    using ReactiveUI;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

    /// <summary>
    /// Установки системных переменных для чертежа
    /// </summary>
    [PublicAPI]
    public static class DocSysVarAuto
    {
        /// <summary>
        /// Системные переменные для установки в чертеж
        /// </summary>
        public static Dictionary<string, object> SysVars = new Dictionary<string, object>();

        internal static void Start()
        {
            // Загрузка системных переменных
            LoadSysVars();

            Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            foreach (Document doc in Application.DocumentManager)
            {
                SetSysVars(doc);
            }
        }

        private static void LoadSysVars()
        {
            SysVars = PikSettings.PikFileSettings.DocSystemVariables ?? new Dictionary<string, object>();
            if (PikSettings.GroupFileSettings.DocSystemVariables != null)
            {
                foreach (var sv in PikSettings.GroupFileSettings.DocSystemVariables)
                {
                    SysVars[sv.Key] = sv.Value;
                }
            }
        }

        private static void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            SetSysVars(e.Document);
        }

        private static void SetSysVars([CanBeNull] Document doc)
        {
            if (doc == null)
                return;
            Logger.Log.Info($"SetSysVars doc={doc.Name}, ActiveDoc={Application.DocumentManager.MdiActiveDocument?.Name}.");
            foreach (var item in SysVars)
            {
                try
                {
                    Logger.Log.Info($"SetSysVars {item.Key}={item.Value}");
                    var val = item.Value;
                    var cVal = item.Key.GetSystemVariable();
                    if (cVal.Equals(val))
                        continue;
                    var itemType = item.Value.GetType();
                    var reqType = cVal.GetType();
                    if (itemType != reqType)
                    {
                        val = Convert.ChangeType(item.Value, reqType);
                    }

                    item.Key.SetSystemVariableTry(val);
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, $"SetSysVars {item.Key}={item.Value}.");
                }
            }
        }
    }
}
