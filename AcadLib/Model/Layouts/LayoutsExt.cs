using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace AcadLib.Layouts
{
    public static class LayoutExt
    {
        [NotNull]
        public static List<Layout> GetLayouts([NotNull] this Database db)
        {
            var layouts = new List<Layout>();
            var dictLayout = db.LayoutDictionaryId.GetObject<DBDictionary>();
            if (dictLayout != null)
            {
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
            }
            return layouts;
        }
    }
}