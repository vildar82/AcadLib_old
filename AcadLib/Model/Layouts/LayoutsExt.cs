using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.Layouts
{
    public static class LayoutExt
    {
        public static List<Layout> GetLayouts(this Database db)
        {
            var layouts = new List<Layout>();
            var dictLayout = db.LayoutDictionaryId.GetObject<DBDictionary>();
            foreach (var entry in dictLayout)
            {
                if (entry.Key != "Model")
                {
                    var layout = entry.Value.GetObject<Layout>();
                    if (layout != null)
                    {
                        layouts.Add(layout);
                    }
                }
            }
            return layouts;
        }
    }
}
