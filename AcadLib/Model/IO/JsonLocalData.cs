namespace AcadLib.IO
{
    using System;
    using System.IO;
    using JetBrains.Annotations;
    using NetLib;

    /// <summary>
    /// Данные хранимые в файле json на сервере, с локальным кэшем
    /// </summary>
    [PublicAPI]
    [Obsolete]
    public class JsonLocalData<T>
    {
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

        [CanBeNull]
        public T Load()
        {
            return !File.Exists(LocalFile) ? default : LocalFile.Deserialize<T>();
        }

        public void Save(T data)
        {
            data.Serialize(LocalFile);
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
            catch
            {
                //
            }
        }
    }
}