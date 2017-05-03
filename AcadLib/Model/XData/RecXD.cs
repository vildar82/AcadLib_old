using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.XData
{
    /// <summary>
    /// Запись XRecord
    /// </summary>
    public class RecXD
    {
        public string Name { get; set; }
        public List<TypedValue> Values { get; set; }

        public RecXD () { }
        public RecXD (string name, List<TypedValue> values)
        {
            Name = name;
            Values = values;
        }

        public bool IsEmpty()
        {
            return Values == null || Values.Count == 0;
        }
    }
}
