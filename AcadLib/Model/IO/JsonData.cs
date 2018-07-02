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
    [Obsolete]
    public class JsonData<T>
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly string LocalFile;

        // ReSharper disable once MemberCanBePrivate.Global
        public readonly string ServerFile;

        /// <summary>
        /// Данные хранимые в файле json на сервере, с локальным кэшем
        /// </summary>
        /// <param name="plugin">Имя плагина</param>
        /// <param name="name">Имя файла - без расширения (всегда .json)</param>
        public JsonData([NotNull] string plugin, [NotNull] string name)
        {
            ServerFile = Path.GetSharedFile(plugin, name + ".json");
            LocalFile = Path.GetUserPluginFile(plugin, name + ".json");
        }

        // ReSharper disable once StaticMemberInGenericType
        private static ILogger Logger { get; } = LogManager.GetCurrentClassLogger();

        [CanBeNull]
        public T Load()
        {
            Copy();
            return !File.Exists(LocalFile) ? default : LocalFile.Deserialize<T>();
        }

        public void Save(T data)
        {
            data.Serialize(ServerFile);
            Copy();
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

        private void Copy()
        {
            if (!File.Exists(ServerFile))
                return;
            try
            {
                File.Copy(ServerFile, LocalFile, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "JsonData копирование файла с сервера локально.");
            }
        }
    }
}