using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.PaletteProps
{
    /// <summary>
    /// 
    /// </summary>
    public class PalettePropsProvider
    {
        private readonly Func<ObjectId[], Document, List<PalettePropsType>> getTypes;

        public PalettePropsProvider(string name, Func<ObjectId[], Document, List<PalettePropsType>> getTypes)
        {
            Name = name;
            this.getTypes = getTypes;
        }

        public string Name { get; set; }

        public List<PalettePropsType> GetTypes(ObjectId[] ids, Document doc)
        {
            return getTypes(ids, doc);
        }
    }
}