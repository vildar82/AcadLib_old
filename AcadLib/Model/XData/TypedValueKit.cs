using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace AcadLib.XData
{
    /// <summary>
    /// Набирает параметры TypedValue
    /// </summary>
    public class TypedValueExtKit
    {
        public TypedValueExtKit()
        {
            Values = new List<TypedValue>();
        }
        public void Add (string name, object value)
        {
            var tvName = TypedValueExt.GetTvExtData(name);
            var tvValue = TypedValueExt.GetTvExtData(value);
            Values.Add(tvName);
            Values.Add(tvValue);
        }             

        public List<TypedValue> Values { get; private set; }
    }
}
