namespace AcadLib.Utils.Tabs
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
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
        private static bool inQuite;
        private static bool isOn;
        [ItemNotNull] [NotNull] private static List<Tab> tabs = new List<Tab>();

        /// <summary>
        /// Воссатановление вкладок
        /// </summary>
        public static void Restore()
        {
            UserSettingsService.ChangeSettings += (o, e) => Init();
            Init();

            // Если восстановление вкладок отключено
            if (isOn)
                return;

            var tabsData = new LocalFileData<Tabs>(GetFile(), false);
            tabsData.TryLoad(() => new Tabs());
            if (tabsData.Data.Drawings == null)
            {
                tabsData.Data.Drawings = new List<string>();
            }

            if (tabsData.Data.Drawings.Any())
            {
                var tabsView = new TabsView(new TabsVM(tabsData.Data));
                if (tabsView.ShowDialog() == true)
                {
                    foreach (var drawing in tabsData.Data.Drawings)
                    {
                        Application.DocumentManager.Open(drawing);
                    }
                }
            }

            Init();
        }

        [NotNull]
        private static string GetFile()
        {
            return Path.GetUserPluginFile(PluginName, PluginName + ".json");
        }

        private static void Init()
        {
            isOn = IsOn();
            if (!isOn)
                return;

            tabs = new List<Tab>();
            foreach (Document doc in Application.DocumentManager)
            {
                AddTab(doc);
            }

            // Если автокад закрывается, то не нужно обрабатывать события закрытия чертежей
            Application.QuitWillStart += (o, e) =>
                inQuite = true;
            Application.BeginQuit += (o, e) =>
            {
                inQuite = true;
                SaveTabs();
            };
            Application.QuitAborted += (o, e) =>
                inQuite = false;

            Commands.Quite += (o, e) =>
                inQuite = false;

            // Подписаться на события открытия/закрытия чертежей
            Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            Application.DocumentManager.DocumentDestroyed += DocumentManager_DocumentDestroyed;
        }

        private static void SaveTabs()
        {
            var tabsData = new LocalFileData<Tabs>(GetFile(), false)
            {
                Data = new Tabs
                {
                    Drawings = tabs.Where(IsValidTab).Select(s => s.Doc.Name).ToList()
                }
            };
            tabsData.TrySave();
        }

        private static bool IsValidTab(Tab tab)
        {
            return tab?.Doc != null && tab.Doc.IsNamedDrawing;
        }

        private static void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            Debug.WriteLine($"DocumentManager_DocumentDestroyed {e?.FileName}");
            if (inQuite)
                return;
        }

        private static void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            Debug.WriteLine($"DocumentManager_DocumentCreated {e?.Document?.Name}");
            AddTab(e?.Document);
        }

        private static void AddTab(Document doc)
        {
            if (doc == null || tabs.Any(d => d.Doc == doc))
                return;
            var tab = new Tab(doc);
            tabs.Add(tab);
            doc.CommandWillStart += Doc_CommandWillStart;
            doc.Database.DatabaseToBeDestroyed += Database_DatabaseToBeDestroyed;
            doc.CloseWillStart += Doc_CloseWillStart;
            doc.BeginDocumentClose += Doc_BeginDocumentClose;
            doc.CloseAborted += Doc_CloseAborted;
            doc.Database.SaveComplete += Database_SaveComplete;
            doc.Database.AbortSave += Database_AbortSave;
            doc.Database.BeginSave += Database_BeginSave;
        }

        private static void Doc_CommandWillStart(object sender, CommandEventArgs e)
        {
            Debug.WriteLine($"Doc_CommandWillStart {e?.GlobalCommandName}.");
        }

        private static void Database_BeginSave(object sender, DatabaseIOEventArgs e)
        {
            Debug.WriteLine($"Database_BeginSave {(sender as Database)?.Filename} {e?.FileName}.");
        }

        private static void Database_AbortSave(object sender, System.EventArgs e)
        {
            Debug.WriteLine($"Database_AbortSave {(sender as Database)?.Filename}.");
        }

        private static void Database_SaveComplete(object sender, DatabaseIOEventArgs e)
        {
            Debug.WriteLine($"Database_SaveComplete {(sender as Database)?.Filename} {e?.FileName}.");
        }

        private static void Doc_CloseAborted(object sender, System.EventArgs e)
        {
            Debug.WriteLine($"Doc_BeginDocumentClose {(sender as Document)?.Name}.");
        }

        private static void Doc_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            Debug.WriteLine($"Doc_BeginDocumentClose {(sender as Document)?.Name}.");
        }

        private static void Doc_CloseWillStart(object sender, System.EventArgs e)
        {
            Debug.WriteLine($"Doc_CloseWillStart {(sender as Document)?.Name}.");
        }

        private static void Database_DatabaseToBeDestroyed(object sender, System.EventArgs e)
        {
            Debug.WriteLine($"Database_DatabaseToBeDestroyed {(sender as Database)?.Filename}.");
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
    }

    internal class Tab
    {
        public Document Doc { get; }

        public Tab(Document doc)
        {
            Doc = doc;
        }
    }
}