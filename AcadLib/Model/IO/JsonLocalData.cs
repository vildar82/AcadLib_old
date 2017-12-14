using JetBrains.Annotations;
using NetLib;
using NLog;
using System;
using System.IO;
using AcadLib.UI.Ribbon.Options;

namespace AcadLib.IO
{
    /// <summary>
    /// Данные хранимые в файле json на сервере, с локальным кэшем
    /// </summary>
    public class JsonLocalData<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static ILogger Logger { get; } = LogManager.GetCurrentClassLogger();
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly string LocalFile;

        /// <summary>
        /// Данные хранимые в файле json на сервере, с локальным кэшем
        /// </summary>
        /// <param name="plugin">Имя плагина</param>
        /// <param name="name">Имя файла - без расширения (всегда .json)</param>
        public JsonLocalData([NotNull] string plugin, [NotNull] string name)
        {
            LocalFile = Path.GetUserPluginFile(plugin, name + ".json");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException"/>
        [CanBeNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public T Load()
        {
            return !File.Exists(LocalFile) ? default : LocalFile.Deserialize<T>();
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

        // ReSharper disable once MemberCanBePrivate.Global
        public void Save(T data)
        {
            data.Serialize(LocalFile);
        }

        public void TrySave(T data)
        {
            try
            {
                Save(data);
            }
            catch
            {
                //
            }
        }
    }
}
