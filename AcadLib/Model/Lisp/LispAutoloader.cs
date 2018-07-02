namespace AcadLib.Lisp
{
    using System.IO;
    using AutoCAD_PIK_Manager.Settings;
    using Autodesk.AutoCAD.ApplicationServices;
    using JetBrains.Annotations;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
    using Path = IO.Path;

    public static class LispAutoloader
    {
        public static void Start()
        {
            Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            foreach (Document doc in Application.DocumentManager)
            {
                LoadLisp(doc);
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
                }
            }
        }
    }
}