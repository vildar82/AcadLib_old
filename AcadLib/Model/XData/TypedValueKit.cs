using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AcadLib.XData
{
    /// <summary>
    /// Набирает параметры TypedValue
    /// </summary>
    [PublicAPI]
    public class TypedValueExtKit
    {
        public List<TypedValue> Values { get; private set; }

        public TypedValueExtKit()
        {
            Values = new List<TypedValue>();
        }

        public void Add(string name, object value)
        {
            var tvName = TypedValueExt.GetTvExtData(name);
            var tvValue = TypedValueExt.GetTvExtData(value);
            Values.Add(tvName);
            Values.Add(tvValue);
        }
    }
}