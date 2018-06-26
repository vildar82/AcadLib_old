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
        private const string ParamRestoreIsOn = "RestoreTabsOn";
        [NotNull]
        private static readonly List<Document> _tabs = new List<Document>();
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

        public static void RestoreTabsIsOn(bool isOn)
        {
            UserSettingsService.SetPluginValue(PluginName, ParamRestoreIsOn, isOn);
            Init();
        }

        private static void Application_Idle(object sender, EventArgs e)
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
                openDrawings = null;
                isOn = true;
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
                NetLib.IO.Path.TryDeleteFile(GetFile());
                return;
            }

            foreach (Document doc in Application.DocumentManager)
            {
                AddTab(doc);
            }

            // Если автокад закрывается, то не нужно обрабатывать события закрытия чертежей
            Application.DocumentManager.DocumentLockModeChanged += DocumentManager_DocumentLockModeChanged;

            // Подписаться на события открытия/закрытия чертежей
            Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            Application.DocumentManager.DocumentDestroyed += DocumentManager_DocumentDestroyed;
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
                case "#QUIT":
                    return;
            }

            cmd = e.GlobalCommandName;
        }

        private static void AddTab(Document doc)
        {
            if (doc?.Database == null)
                return;
            _tabs.Add(doc);
            doc.Database.SaveComplete += (o, e) =>
            {
                if (cmd != "QUIT")
                    SaveTabs();
            };
            SaveTabs();
        }

        private static void RemoveTabs()
        {
            _tabs.RemoveAll(t => t?.Database == null);
            SaveTabs();
        }

        private static void SaveTabs()
        {
            if (!isOn)
                return;
            Debug.WriteLine("SaveTabs");
            var drawings = _tabs.Where(w => w?.Database != null && w.IsNamedDrawing).Select(s => s.Name).ToList();
            var tabsData = new LocalFileData<Tabs>(GetFile(), false) { Data = new Tabs { Drawings = drawings } };
            tabsData.TrySave();
        }

        private static void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            Debug.WriteLine($"DocumentManager_DocumentDestroyed cmd={cmd}");
            if (cmd == "CLOSE")
                RemoveTabs();
        }

        private static void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            AddTab(e?.Document);
        }

        private static bool IsOn()
        {
            if (!UserSettingsService.IsPreviewUpdate)
            {
                UserSettingsService.RemovePlugin(PluginName);
                return false;
            }

            var pluginSettings = UserSettingsService.GetPluginSettings(PluginName);
            if (pluginSettings == null)
            {
                AddRestoreProperty(UserSettingsService.AddPluginSettings(PluginName));
            }
            else
            {
                var propRestore = UserSettingsService.GetPluginProperty(PluginName, ParamRestoreIsOn);
                if (propRestore == null)
                {
                    AddRestoreProperty(pluginSettings);
                }
            }

            void AddRestoreProperty(PluginSettings plugin)
            {
                plugin.Add(
                    ParamRestoreIsOn,
                    "Восстановление вкладок",
                    "При старте автокада открывать ранее открытые вкладки чертежей.",
                    true);
            }

            return UserSettingsService.GetPluginValue<bool>(PluginName, ParamRestoreIsOn);
        }
    }
}