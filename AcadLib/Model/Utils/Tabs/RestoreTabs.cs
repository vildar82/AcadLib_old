namespace AcadLib.Utils.Tabs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Autodesk.AutoCAD.ApplicationServices;
    using Data;
    using IO;
    using JetBrains.Annotations;
    using NetLib;
    using UI;
    using User;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

    /// <summary>
    /// Восстановление ранее отурытых вкладок
    /// </summary>
    public static class RestoreTabs
    {
        private const string PluginName = "RestoreTabs";
        private const string ParamIsOn = "RestoreTabsOn";
        private static string cmd;
        private static bool isOn;
        private static List<string> openDrawings;

        /// <summary>
        /// Воссатановление вкладок
        /// </summary>
        public static void Restore()
        {
            var tabsData = new LocalFileData<Tabs>(GetFile(), false);
            tabsData.TryLoad(() => new Tabs());

            UserSettingsService.ChangeSettings += (o, e) => Init();
            Init();

            // Если восстановление вкладок отключено
            if (!isOn)
                return;

            if (tabsData.Data.Drawings?.Any() == true)
            {
                var openedDraws = Application.DocumentManager.Cast<Document>().Where(w => w.IsNamedDrawing)
                    .Select(s => s.Name).ToList();
                var tabVM = new TabsVM(tabsData.Data.Drawings.Except(openedDraws, StringComparer.OrdinalIgnoreCase));
                var tabsView = new TabsView(tabVM);
                if (tabsView.ShowDialog() == true)
                {
                    openDrawings = tabVM.Tabs.Where(w => w.Restore).Select(s => s.File).ToList();
                    Application.Idle += Application_Idle;
                }
            }
        }

        private static void Application_Idle(object sender, System.EventArgs e)
        {
            Application.Idle -= Application_Idle;
            if (openDrawings?.Any() != true)
                return;
            try
            {
                isOn = false;
                foreach (var drawing in openDrawings)
                {
                    Application.DocumentManager.Open(drawing, false);
                }
            }
            finally
            {
                isOn = true;
                SaveTabs();
            }
        }

        private static void Init()
        {
            isOn = IsOn();
            if (!isOn)
            {
                Application.DocumentManager.DocumentCreated -= DocumentManager_DocumentCreated;
                Application.DocumentManager.DocumentDestroyed -= DocumentManager_DocumentDestroyed;
                Application.DocumentManager.DocumentLockModeChanged -= DocumentManager_DocumentLockModeChanged;
                return;
            }

            // Если автокад закрывается, то не нужно обрабатывать события закрытия чертежей
            Application.DocumentManager.DocumentLockModeChanged += DocumentManager_DocumentLockModeChanged;

            // Подписаться на события открытия/закрытия чертежей
            Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            Application.DocumentManager.DocumentDestroyed += DocumentManager_DocumentDestroyed;

            SaveTabs();
        }

        [NotNull]
        private static string GetFile()
        {
            return Path.GetUserPluginFile(PluginName, PluginName + ".json");
        }

        private static void DocumentManager_DocumentLockModeChanged(object sender, [NotNull] DocumentLockModeChangedEventArgs e)
        {
            Debug.WriteLine($"DocumentManager_DocumentLockModeChanged - {e.GlobalCommandName}.");
            switch (e.GlobalCommandName)
            {
                case "":
                case "#":
                case "#CLOSE":
                    return;
            }

            cmd = e.GlobalCommandName;
        }

        private static void SaveTabs()
        {
            if (!isOn)
                return;
            Debug.WriteLine("SaveTabs");
            var drawings = new List<string>();
            foreach (Document doc in Application.DocumentManager)
            {
                if (doc.Database == null)
                    continue;
                    doc.Database.SaveComplete += (o, e) => SaveTabs();
                if (doc.IsNamedDrawing)
                    drawings.Add(doc.Name);
            }

            var tabsData = new LocalFileData<Tabs>(GetFile(), false) { Data = new Tabs { Drawings = drawings } };
            tabsData.TrySave();
        }

        private static void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            Debug.WriteLine($"DocumentManager_DocumentDestroyed cmd={cmd}");
            if (cmd == "CLOSE")
                SaveTabs();
        }

        private static void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            SaveTabs();
        }

        private static bool IsOn()
        {
            var pluginSettings = UserSettingsService.GetPluginSettings(PluginName);
            if (pluginSettings == null)
            {
                UserSettingsService.AddPluginSettings(PluginName).Add(
                    ParamIsOn,
                    "Восстановление вкладок",
                    "При старте автокада открывать ранее открытые вкладки чертежей.",
                    true);
            }

            return UserSettingsService.GetPluginValue<bool>(PluginName, ParamIsOn);
        }

        public static void RestoreTabsIsOn(bool isOn)
        {
            UserSettingsService.SetPluginValue(PluginName, ParamIsOn, isOn);
            Init();
        }
    }
}