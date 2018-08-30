namespace AcadLib.Utils.Tabs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using AcadLib.UI.StatusBar;
    using Autodesk.AutoCAD.ApplicationServices;
    using Data;
    using Errors;
    using JetBrains.Annotations;
    using NetLib;
    using Properties;
    using UI;
    using User;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
    using Path = IO.Path;

    /// <summary>
    /// Восстановление ранее отурытых вкладок
    /// </summary>
    public static class RestoreTabs
    {
        internal const string PluginName = "RestoreTabs";
        private const string ParamRestoreIsOn = "RestoreTabsIsOn";
        [NotNull]
        private static readonly List<Document> _tabs = new List<Document>();
        private static string cmd;
        private static List<string> restoreTabs;

        public static void Init()
        {
            try
            {
                UserSettingsService.RegPlugin(PluginName, CreateUserSettings, CheckUserSettings);
                UserSettingsService.ChangeSettings += UserSettingsService_ChangeSettings;

                // Добавление кнопки в статус бар
                StatusBarEx.AddPane(string.Empty, "Откытие чертежей", (p, e) => Restore(), icon: Resources.restoreFiles16);

                var isOn = UserSettingsService.GetPluginValue<bool>(PluginName, ParamRestoreIsOn);
                if (isOn)
                {
                    Subscribe();
                    var tabsData = new LocalFileData<Tabs>(GetFile(), false);
                    tabsData.TryLoad(() => new Tabs());
                    if (tabsData.Data?.Drawings?.Count > 0)
                    {
                        Restore();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "RestoreTabs.Init");
            }
        }

        private static PluginSettings CreateUserSettings()
        {
            return new PluginSettings
            {
                Name = PluginName,
                Title = "Восстановление вкладок",
                Properties = new List<UserProperty>
                {
                    new UserProperty
                    {
                        ID = ParamRestoreIsOn,
                        Name = "Запускать при старте",
                        Value = true,
                        Description = "Открывать окно открытия чертежей последнего сеанса при старте автокада."
                    }
                }
            };
        }

        private static void CheckUserSettings(PluginSettings pluginSettings)
        {
            pluginSettings.Title = "Восстановление вкладок";
            var propIsOn = pluginSettings.Properties.FirstOrDefault(p => p.ID == ParamRestoreIsOn) ?? new UserProperty
            {
                ID = ParamRestoreIsOn,
                Value = true,
            };
            propIsOn.Name = "Запускать при старте";
            propIsOn.Description = "Открывать окно открытия чертежей последнего сеанса при старте автокада.";
            pluginSettings.Properties = new List<UserProperty> { propIsOn };
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
                var tabVM = new TabsVM(restoreTabs);
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
            var isOn = UserSettingsService.GetPluginValue<bool>(PluginName, ParamRestoreIsOn);
            if (isOn)
            {
                Subscribe();
            }
            else
            {
                Unsubscribe();
            }
        }

        private static void Subscribe()
        {
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

        private static void Unsubscribe()
        {
            try
            {
                Application.DocumentManager.DocumentLockModeChanged -= DocumentManager_DocumentLockModeChanged;
                Application.DocumentManager.DocumentCreated -= DocumentManager_DocumentCreated;
                Application.DocumentManager.DocumentDestroyed -= DocumentManager_DocumentDestroyed;

                foreach (var tab in _tabs)
                {
                    if (tab?.Database != null)
                        tab.Database.SaveComplete -= Database_SaveComplete;
                }

                _tabs.Clear();
            }
            catch
            {
                // Если подписок не было
            }
        }

        [NotNull]
        private static string GetFile()
        {
            return Path.GetUserPluginFile(PluginName, PluginName + ".json");
        }

        private static void DocumentManager_DocumentLockModeChanged(object sender, [NotNull] DocumentLockModeChangedEventArgs e)
        {
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
            if (doc.IsNamedDrawing)
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
            Debug.WriteLine("SaveTabs");
            var drawings = _tabs.Where(w => w?.Database != null && w.IsNamedDrawing).Select(s => s.Name).ToList();
            var tabsData = new LocalFileData<Tabs>(GetFile(), false) { Data = new Tabs { Drawings = drawings } };
            tabsData.TrySave();
        }

        private static void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            Debug.WriteLine($"DocumentManager_DocumentDestroyed cmd={cmd}");
            if (cmd == "CLOSE" && System.IO.Path.IsPathRooted(e.FileName))
            {
                RemoveTabs();
            }
        }

        private static void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            AddTab(e?.Document);
        }

        public static void SetIsOn(bool isOn)
        {
            UserSettingsService.SetPluginValue(PluginName, ParamRestoreIsOn, isOn);
        }

        public static bool GetIsOn()
        {
            return UserSettingsService.GetPluginValue<bool>(PluginName, ParamRestoreIsOn);
        }
    }
}