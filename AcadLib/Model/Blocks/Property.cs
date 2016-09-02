using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Blocks
{
    public enum PropertyType
    {
        /// <summary>
        /// Не установлено
        /// </summary>
        None,
        /// <summary>
        /// Аттрибут
        /// </summary>
        Attribute,
        /// <summary>
        /// Динамическое свойство
        /// </summary>
        Dynamic
    }

    /// <summary>
    /// Свойства динамического блока
    /// </summary>
    public class Property : IEquatable<Property>
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public PropertyType Type { get; set; }
        /// <summary>
        /// Только, если тип параматера - атрибут!
        /// </summary>
        public ObjectId IdAtrRef { get; set; }

        public Property(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public Property(string name, object value, ObjectId idAtrRef)
        {
            Name = name;
            Value = value;
            IdAtrRef = idAtrRef;
            Type = PropertyType.Attribute;
        }

        public Property(string name, object value, PropertyType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Все видимые атрибуты и динамические свойства блока
        /// </summary>        
        public static List<Property> GetAllProperties (BlockReference blRef)
        {
            List<Property> props = new List<Property>();
            var attrs = AttributeInfo.GetAttrRefs(blRef);
            foreach (var atr in attrs)
            {
                Property prop = new Property(atr.Tag, atr.Text.Trim(), atr.IdAtr);
                props.Add(prop);
            }
            props.AddRange(GetDynamicProperties(blRef));
            return props;
        }

        /// <summary>
        /// Динамические свойства блока
        /// </summary>        
        public static List<Property> GetDynamicProperties (BlockReference blRef)
        {
            List<Property> props = new List<Property>();
            if (blRef.DynamicBlockReferencePropertyCollection!= null)
            {
                foreach (DynamicBlockReferenceProperty dyn in blRef.DynamicBlockReferencePropertyCollection)
                {
                    if (dyn.VisibleInCurrentVisibilityState)
                    {
                        if (dyn.PropertyName.Equals("Origin", StringComparison.OrdinalIgnoreCase)) continue;
                        Property prop = new Property(dyn.PropertyName, dyn.Value, PropertyType.Dynamic);
                        props.Add(prop);
                    }
                }
            }
            return props;
        }

        public bool Equals (Property other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            var res = Name == other.Name;
            return res;
        }

        public override int GetHashCode ()
        {
            return Name.GetHashCode();
        }
    }
}
