using JetBrains.Annotations;
using NetLib;
using System.IO;

namespace AcadLib.IO
{
    /// <summary>
    /// Данные хранимые в файле json на сервере, с локальным кэшем
    /// </summary>
    [PublicAPI]
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

        // ReSharper disable once MemberCanBePrivate.Global
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