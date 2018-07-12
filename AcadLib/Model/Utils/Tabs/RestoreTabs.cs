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
    using General = AcadLib.General;

    /// <summary>
    /// Восстановление ранее отурытых вкладок
    /// </summary>
    public static class RestoreTabs
    {
        internal const string PluginName = "RestoreTabs";
        private const string ParamRestoreIsOn = "RestoreTabsOn";
        [NotNull] private static readonly List<Document> _tabs = new List<Document>();
        private static string cmd;
        private static bool isOn;
        private static List<string> restoreTabs;

        public static void Init()
        {
            try
            {
                isOn = IsOn();

                // Добавление кнопки в статус бар
                StatusBarEx.AddPane(string.Empty, "Откытие чертежей", (p, e) => Restore(), icon: Resources.restoreFiles16);
                if (isOn)
                {
                    Restore();
                }

                Subscribe();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "RestoreTabs.Init");
            }
        }

        public static void RestoreTabsIsOn(bool isOn)
        {
            UserSettingsService.SetPluginValue(PluginName, ParamRestoreIsOn, isOn);
            Subscribe();
        }

        /// <summary>
        /// Воссатановление вкладок
        /// </summary>
        private static void Restore()
        {
            var tabsData = new LocalFileData<Tabs>(GetFile(), false);
            tabsData.TryLoad(() => new Tabs());
            restoreTabs = tabsData.Data?.Drawings;
            Application.Idle += Application_Idle;
        }

        private static void Application_Idle(object sender, EventArgs e)
        {
            try
            {
                Application.Idle -= Application_Idle;
                UserSettingsService.ChangeSettings -= UserSettingsService_ChangeSettings;
                UserSettingsService.ChangeSettings += UserSettingsService_ChangeSettings;
                var openedDraws = new List<string>();
                if (restoreTabs?.Any() == true)
                {
                    // Сохранить список чертежей, на случай если это окно пропустят и закроют автокад.
                    var tabsData = new LocalFileData<Tabs>(GetFile(), false) { Data = new Tabs { Drawings = restoreTabs } };
                    tabsData.TrySave();
                    var docs = Application.DocumentManager.Cast<Document>().ToList();
                    foreach (var doc in docs)
                    {
                        if (doc.IsNamedDrawing)
                        {
                            openedDraws.Add(doc.Name);
                        }
                    }
                }

                if (!AcadHelper.IsOneAcadRun())
                {
                    Debug.WriteLine("RestoreTabs. Запущено несколько автокадов.");
                    restoreTabs = new List<string>();
                }

                restoreTabs = restoreTabs?.Except(openedDraws, StringComparer.OrdinalIgnoreCase).ToList() ?? new List<string>();
                var tabVM = new TabsVM(restoreTabs, isOn);
                var tabsView = new TabsView(tabVM);
                if (tabsView.ShowDialog() == true)
                {
                    try
                    {
                        Statistic.PluginStatisticsHelper.PluginStart("OpenRestoreTabs");
                        var closeDocs = Application.DocumentManager.Cast<Document>().Where(w => !w.IsNamedDrawing).ToList();
                        var tabs = tabVM.Tabs.Where(w => w.Restore).Select(s => s.File).ToList();
                        if (tabVM.HasHistory)
                        {
                            tabs = tabs.Union(tabVM.History.Where(w => w.Restore).Select(s => s.File)).Distinct().ToList();
                        }

                        foreach (var item in tabs)
                        {
                            try
                            {
                                Application.DocumentManager.Open(item, false);
                            }
                            catch (Exception ex)
                            {
                                Inspector.AddError($"Ошибка открытия файла '{item}' - {ex.Message}");
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
                        Inspector.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "RestoreTabs.Application_Idle");
            }
        }

        private static void UserSettingsService_ChangeSettings(object sender, EventArgs e)
        {
            Subscribe();
        }

        private static void Subscribe()
        {
            if (!isOn)
            {
                Application.DocumentManager.DocumentCreated -= DocumentManager_DocumentCreated;
                Application.DocumentManager.DocumentDestroyed -= DocumentManager_DocumentDestroyed;
                Application.DocumentManager.DocumentLockModeChanged -= DocumentManager_DocumentLockModeChanged;
                NetLib.IO.Path.TryDeleteFile(GetFile());
                _tabs.Clear();
                return;
            }

            foreach (Document doc in Application.DocumentManager)
            {
                AddTab(doc);
            }

            // Если автокад закрывается, то не нужно обрабатывать события закрытия чертежей
            Application.DocumentManager.DocumentLockModeChanged -= DocumentManager_DocumentLockModeChanged;
            Application.DocumentManager.DocumentLockModeChanged += DocumentManager_DocumentLockModeChanged;

            // Подписаться на события открытия/закрытия чертежей
            Application.DocumentManager.DocumentCreated -= DocumentManager_DocumentCreated;
            Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            Application.DocumentManager.DocumentDestroyed -= DocumentManager_DocumentDestroyed;
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
            if (doc?.Database == null || _tabs.Contains(doc))
                return;
            _tabs.Add(doc);
            doc.Database.SaveComplete -= Database_SaveComplete;
            doc.Database.SaveComplete += Database_SaveComplete;
            SaveTabs();
        }

        private static void Database_SaveComplete(object sender, Autodesk.AutoCAD.DatabaseServices.DatabaseIOEventArgs e)
        {
            if (cmd != "QUIT")
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