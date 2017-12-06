using JetBrains.Annotations;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace AcadLib.Files
{
    public class SerializerXml
    {
        private readonly string _settingsFile;

        public SerializerXml(string settingsFile)
        {
            _settingsFile = settingsFile;
        }

        public void SerializeList<T>([NotNull] T settings)
        {
            using (var fs = new FileStream(_settingsFile, FileMode.Create, FileAccess.Write))
            {
                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(fs, settings);
            }
        }

        public void SerializeList<T>([NotNull] T settings, [NotNull] params Type[] types)
        {
            using (var fs = new FileStream(_settingsFile, FileMode.Create, FileAccess.Write))
            {
                var ser = new XmlSerializer(typeof(T), types);
                ser.Serialize(fs, settings);
            }
        }

        public T DeserializeXmlFile<T>()
        {
            var ser = new XmlSerializer(typeof(T));
            using (var reader = XmlReader.Create(_settingsFile))
            {
                return (T)ser.Deserialize(reader);
            }
        }

        public T DeserializeXmlFile<T>([NotNull] params Type[] types)
        {
            var ser = new XmlSerializer(typeof(T), types);
            using (var reader = XmlReader.Create(_settingsFile))
            {
                return (T)ser.Deserialize(reader);
            }
        }

        /// <summary>
        /// Считывание объекта из файла
        /// </summary>
        /// <typeparam name="T">Тип считываемого объекта></typeparam>
        /// <param name="file">Файл xml</param>
        /// <returns>Объект T или null</returns>
        [CanBeNull]
        public static T Load<T>(string file) where T : class, new()
        {
            var ser = new SerializerXml(file);
            T res = null;
            try
            {
                res = ser.DeserializeXmlFile<T>();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"Ошибка десирилизации объекта {typeof(T)} из файла {file}");
                //res = new T();
            }
            return res;
        }

        [CanBeNull]
        public static T Load<T>(string file, params Type[] types) where T : class, new()
        {
            var ser = new SerializerXml(file);
            T res = null;
            try
            {
                res = ser.DeserializeXmlFile<T>(types);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"Ошибка десирилизации объекта {typeof(T)} из файла {file}");
                //res = new T();
            }
            return res;
        }

        /// <summary>
        /// Сохранение объекта в файл.
        /// При ошибке записывается лог.
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="file">Файл</param>
        /// <param name="obj">Объект</param>
        public static void Save<T>(string file, T obj)
        {
            var ser = new SerializerXml(file);
            try
            {
                ser.SerializeList(obj);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"Ошибка сирилизации объекта {typeof(T)} в файл {file}");
            }
        }

        public static void Save<T>(string file, T obj, params Type[] types)
        {
            var ser = new SerializerXml(file);
            try
            {
                ser.SerializeList(obj, types);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"Ошибка сирилизации объекта {typeof(T)} в файл {file}");
            }
        }
    }
}