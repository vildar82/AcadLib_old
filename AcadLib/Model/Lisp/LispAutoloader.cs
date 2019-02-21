namespace AcadLib.Lisp
{
    using System.Collections.Generic;
    using System.IO;
    using AutoCAD_PIK_Manager.Settings;
    using Autodesk.AutoCAD.ApplicationServices;
    using JetBrains.Annotations;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
    using Path = IO.Path;

    public static class LispAutoloader
    {
        private static List<string> lispFiles;

        public static void Start()
        {
            lispFiles = new List<string>();
            if (PikSettings.PikFileSettings?.AutoLoadLispPathBySettings?.Count > 0)
            {
                lispFiles = PikSettings.PikFileSettings.AutoLoadLispPathBySettings;
            }

            if (PikSettings.GroupFileSettings?.AutoLoadLispPathBySettings?.Count > 0)
            {
                lispFiles.AddRange(PikSettings.GroupFileSettings.AutoLoadLispPathBySettings);
            }

            // Для удаленщиков грузить лисп оптимизацйи
            if (General.IsRemoteUser())
            {
                var lspRemote = Path.GetLocalSettingsFile(@"Script\Lisp\OptimiseVarRemote.lsp");
                if (!File.Exists(lspRemote))
                {
                    Logger.Log.Warn($"Не найден лисп файл для оптимизации работы удоленщика - {lspRemote}.");
                }
                else
                {
                    Logger.Log.Info($"Добавлен лисп файл для оптимизации работы удаленщика - {lspRemote}");
                    lispFiles.Add(lspRemote);
                }
            }
            else
            {
                Logger.Log.Info("Пропущен лисп файл для оптимизации работы удаленщика. Это не удаленщик!");
            }

            if (lispFiles.Count > 0)
            {
                Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
                foreach (Document doc in Application.DocumentManager)
                {
                    LoadLisp(doc);
                }
            }
            else
            {
                lispFiles = null;
            }
        }

        private static void DocumentManager_DocumentCreated(object sender, [CanBeNull] DocumentCollectionEventArgs e)
        {
            LoadLisp(e?.Document);
        }

        private static void LoadLisp([CanBeNull] Document doc)
        {
            if (doc == null)
                return;
            foreach (var refLisp in PikSettings.PikFileSettings.AutoLoadLispPathBySettings)
            {
                var startupLispFile = Path.GetLocalSettingsFile(refLisp);
                if (File.Exists(startupLispFile))
                {
                    var lspPath = startupLispFile.Replace('\\', '/');
                    doc.SendStringToExecute($"(load \"{lspPath}\") ", true, false, true);
                    Logger.Log.Info($"LispAutoloader Загружен лисп {refLisp}.");
                }
                else
                {
                    Logger.Log.Info($"LispAutoloader Не найден лисп {refLisp}.");
                }
            }
        }
    }
}
