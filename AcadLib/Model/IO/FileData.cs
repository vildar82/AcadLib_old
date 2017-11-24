using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetLib;
using NLog;

namespace AcadLib.IO
{
    /// <summary>
    /// Данные хранимые в файле json на сервере, с локальным кэшем
    /// </summary>
    public class JsonData<T>
    {
        private static NLog.Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private readonly string serverFile;
        private readonly string localFile;

        public JsonData(string plugin, string name)
        {
            serverFile = Path.GetSharedFile(plugin, name + ".json");
            localFile = Path.GetUserPluginFile(plugin, name + ".json");
        }

        public T Load()
        {
            Copy();
            return localFile.Deserialize<T>();
        }

        public void Save(T data)
        {
            data.Serialize(serverFile);
        }

        private void Copy()
        {
            try
            {
                File.Copy(serverFile, localFile, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "JsonData копирование файла с сервера локально.");
            }
        }
    }
}
