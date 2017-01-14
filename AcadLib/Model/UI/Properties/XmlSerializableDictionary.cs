using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AcadLib.UI.Properties
{
    /// <summary>
    /// Класс словаря сериализуемый в xml
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [XmlRoot("Dictionary")]
    public class XmlSerializableDictionary<TValue>
            : Dictionary<string, TValue>, IXmlSerializable
    {
        public XmlSerializableDictionary () : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public XmlSerializableDictionary(Dictionary<string, TValue> dict) : base(StringComparer.OrdinalIgnoreCase)
        {
            foreach (var item in dict)
            {
                Add(item.Key, item.Value);
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema ()
        {
            return null;
        }

        public void ReadXml (System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(string));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                string key = (string)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml (System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(string));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (string key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }
}
