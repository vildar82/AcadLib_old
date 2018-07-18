namespace AcadLib.IO
{
    using System;
    using System.IO;
    using JetBrains.Annotations;
    using NetLib;
    using NLog;

    /// <summary>
    /// Данные хранимые в файле json на сервере, с локальным кэшем
    /// </summary>
    [PublicAPI]
    public class JsonData<T>
        where T : class, new()
    {
        private FileData<T> fileData;

        /// <summary>
        /// Данные хранимые в файле json на сервере, с локальным кэшем
        /// </summary>
        /// <param name="plugin">Имя плагина</param>
        /// <param name="name">Имя файла - без расширения (всегда .json)</param>
        public JsonData([NotNull] string plugin, [NotNull] string name)
        {
            var serverFile = Path.GetSharedFile(plugin, name + ".json");
            var localFile = Path.GetUserPluginFile(plugin, name + ".json");
            fileData = new FileData<T>(serverFile, localFile, false);
        }

        private static ILogger Logger { get; } = LogManager.GetCurrentClassLogger();

        [CanBeNull]
        public T Load()
        {
            fileData.Load();
            return fileData.Data;
        }

        public void Save(T data)
        {
            fileData.Data = data;
            fileData.Save();
        }

        [CanBeNull]
        public T TryLoad()
        {
            try
            {
                return Load();
            }
            catch
            {
                return default;
            }
        }

        public void TrySave(T data)
        {
            try
            {
                Save(data);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}