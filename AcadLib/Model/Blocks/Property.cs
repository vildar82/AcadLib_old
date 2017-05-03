using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Blocks
{
    

    /// <summary>
    /// Свойства динамического блока
    /// </summary>
    public class Property : IEquatable<Property>, ICloneable
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public PropertyType Type { get; set; }
        /// <summary>
        /// Видит ли пользователь это свойство
        /// </summary>
        public bool IsShow { get; set; }
        public bool IsReadOnly { get; set; } = false;
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
            IsShow = true;
        }

        public Property(string name, object value, PropertyType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        public Property(DynamicBlockReferenceProperty dynProp)
        {
            Name = dynProp.PropertyName;
            Value = dynProp.Value;
            Type = PropertyType.Dynamic;
            IsShow = dynProp.Show;
            IsReadOnly = dynProp.ReadOnly;
        }

        /// <summary>
        /// Все видимые атрибуты и динамические свойства блока
        /// </summary>        
        public static List<Property> GetAllProperties (BlockReference blRef)
        {
            var props = new List<Property>();
            var attrs = AttributeInfo.GetAttrRefs(blRef);
            foreach (var atr in attrs)
            {
                var prop = new Property(atr.Tag, atr.Text.Trim(), atr.IdAtr);                
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
            var props = new List<Property>();
            if (blRef.DynamicBlockReferencePropertyCollection!= null)
            {
                foreach (DynamicBlockReferenceProperty dyn in blRef.DynamicBlockReferencePropertyCollection)
                {
                    if (dyn.VisibleInCurrentVisibilityState)
                    {
                        if (dyn.PropertyName.Equals("Origin", StringComparison.OrdinalIgnoreCase)) continue;
                        var prop = new Property(dyn);                        
                        props.Add(prop);
                    }
                }
            }
            return props;
        }

        private bool EqualValue(object value)
        {            
            if (Value is double && value is double)
            {
                return Math.Abs((double)Value - (double)value) < 0.0001;
            }
            return Value.Equals(value);
        }

        public bool Equals (Property other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            var res = Name == other.Name && EqualValue(other.Value);

            return res;
        }        

        public override int GetHashCode ()
        {
            return Name.GetHashCode();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
