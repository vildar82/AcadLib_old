namespace AcadLib.Utils.Tabs.UI.History
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IO;
    using JetBrains.Annotations;
    using NetLib;

    public static class HistoryModel
    {
        [ItemNotNull]
        [NotNull]
        public static List<HistoryTab> LoadHistoryCache()
        {
            var data = new LocalFileData<List<HistoryTab>>(GetHistoryFile(), false);
            data.TryLoad(() => new List<HistoryTab>());
            return data.Data ?? new List<HistoryTab>();
        }

        public static void SaveHistoryCache(List<HistoryTab> historyTabs)
        {
            if (historyTabs?.Any() == true)
            {
                var data = new LocalFileData<List<HistoryTab>>(GetHistoryFile(), false) { Data = historyTabs };
                data.TrySave();
            }
        }

        [NotNull]
        private static string GetHistoryFile()
        {
            return Path.GetUserPluginFile(RestoreTabs.PluginName, "HistoryTabs.json");
        }
    }
}