using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Files;
using AutoCAD_PIK_Manager;

namespace AcadLib.NET
{
   [Serializable]
   public abstract class OptionsAbstract
   {
      protected readonly string fileOptions;
      protected static OptionsAbstract instance;      

      private OptionsAbstract ()
      { }

      protected OptionsAbstract(string file)
      {
         fileOptions = file;
      }

      public static OptionsAbstract Instance
      {
         get
         {
            if (instance == null)
            {
               instance = load();
            }
            return instance;
         }
      }      

      public void Save()
      {
         try
         {
            if (!File.Exists(fileOptions))
            {
               Directory.CreateDirectory(Path.GetDirectoryName(fileOptions));
            }
            SerializerXml xmlSer = new SerializerXml(fileOptions);
            xmlSer.SerializeList(Instance);
         }
         catch (Exception ex)
         {
            Log.Error(ex, $"Не удалось сериализовать настройки в {fileOptions}");
         }
      }

      public abstract OptionsAbstract DefaultOptions();

      protected static OptionsAbstract load(string file)
      {
         OptionsAbstract options = null;
         // загрузка из файла настроек
         if (File.Exists(file))
         {
            SerializerXml xmlSer = new SerializerXml(file);
            try
            {
               options = xmlSer.DeserializeXmlFile<OptionsAbstract>();
               if (options != null)
               {                 
                  return options;
               }
            }
            catch (Exception ex)
            {
               Log.Error(ex, "Не удалось десериализовать настройки из файла {0}", file);
            }
         }
         return defaultOptions();
      }     
   }
}
