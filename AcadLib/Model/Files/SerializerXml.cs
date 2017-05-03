using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace AcadLib.Files
{
    public class SerializerXml
    {
        private string _settingsFile;

        public SerializerXml (string settingsFile)
        {
            _settingsFile = settingsFile;
        }

        public void SerializeList<T> (T settings)
        {
            using (var fs = new FileStream(_settingsFile, FileMode.Create, FileAccess.Write))
            {
                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(fs, settings);
            }
        }

        public T DeserializeXmlFile<T> ()
        {
            var ser = new XmlSerializer(typeof(T));
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
        public static T Load<T> (string file) where T: class, new()
        {
            var ser = new SerializerXml(file);
            T res = null;
            try
            {
                res = ser.DeserializeXmlFile<T>();
            }            
            catch(Exception ex)
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
        public static void Save<T> (string file, T obj)
        {
            var ser = new SerializerXml(file);
            try
            {
                ser.SerializeList(obj);
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex, $"Ошибка сирилизации объекта {typeof(T)} в файл {file}");
            }
        }             
    }
}