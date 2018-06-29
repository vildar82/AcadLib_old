namespace AcadLib.Utils.Tabs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using AcadLib.UI.StatusBar;
    using Autodesk.AutoCAD.ApplicationServices;
    using Data;
    using Errors;
    using IO;
    using JetBrains.Annotations;
    using NetLib;
    using Properties;
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

        public static void Init()
        {
            // Добавление кнопки в статус бар
            StatusBarEx.AddPane(string.Empty, "Откытие чертежей", (p, e) => Restore(), icon: Resources.restoreFiles16);
            Restore();
        }

        /// <summary>
        /// Воссатановление вкладок
        /// </summary>
        public static void Restore()
        {
            Application.Idle += Application_Idle;
        }

        public static void RestoreTabsIsOn(bool isOn)
        {
            UserSettingsService.SetPluginValue(PluginName, ParamRestoreIsOn, isOn);
            Subscribe();
        }

        private static void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= Application_Idle;
            var tabsData = new LocalFileData<Tabs>(GetFile(), false);
            tabsData.TryLoad(() => new Tabs());

            UserSettingsService.ChangeSettings -= UserSettingsService_ChangeSettings;
            UserSettingsService.ChangeSettings += UserSettingsService_ChangeSettings;
            Subscribe();

            // Если восстановление вкладок отключено
            if (!isOn)
                return;

            if (tabsData.Data.Drawings?.Any() == true)
            {
                tabsData.TrySave();
                var docs = Application.DocumentManager.Cast<Document>().ToList();
                var openedDraws = new List<string>();
                foreach (var doc in docs)
                {
                    if (doc.IsNamedDrawing)
                    {
                        openedDraws.Add(doc.Name);
                    }
                }

                var tabVM = new TabsVM(tabsData.Data.Drawings.Except(openedDraws, StringComparer.OrdinalIgnoreCase));
                var tabsView = new TabsView(tabVM);
                if (tabsView.ShowDialog() == true)
                {
                    var oldIsOn = isOn;
                    try
                    {
                        var closeDocs = Application.DocumentManager.Cast<Document>().Where(w => !w.IsNamedDrawing).ToList();
                        isOn = false;
                        foreach (var item in tabVM.Tabs.Where(w => w.Restore))
                        {
                            try
                            {
                                Application.DocumentManager.Open(item.File, false);
                            }
                            catch (Exception ex)
                            {
                                Inspector.AddError($"Ошибка открытия файла '{item.File}' - {ex.Message}");
                            }
                        }

                        // Закрыть пустые чертежи
                        foreach (var doc in closeDocs)
                        {
                            try
                            {
                                doc.CloseAndDiscard();
                            }
                            catch (Exception ex)
                            {
                                Logger.Log.Error("RestoreTabs. Закрыть пустые чертежи.", ex);
                            }
                        }
                    }
                    finally
                    {
                        isOn = oldIsOn;
                    }
                }
            }
        }

        private static void UserSettingsService_ChangeSettings(object sender, EventArgs e)
        {
            Subscribe();
        }

        private static void Subscribe()
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