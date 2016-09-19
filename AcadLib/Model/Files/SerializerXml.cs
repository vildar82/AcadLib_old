using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AcadLib.Files
{
   public class SerializerXml
   {
      private string _settingsFile;

      public SerializerXml(string settingsFile)
      {
         _settingsFile = settingsFile;
      }

      public void SerializeList<T>(T settings)
      {
         using (FileStream fs = new FileStream(_settingsFile, FileMode.Create, FileAccess.Write))
         {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            ser.Serialize(fs, settings);
         }
      }

      public T DeserializeXmlFile<T>()
      {
         XmlSerializer ser = new XmlSerializer(typeof(T));
         using (XmlReader reader = XmlReader.Create(_settingsFile))
         {
            return (T)ser.Deserialize(reader);
         }
      }
   }
}
