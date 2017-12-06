using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace AcadLib.Blocks
{
    /// <summary>
    /// Описание AttributeReference для хранения
    /// Так же подходит для AttributeDefinition
    /// </summary>
    [Obsolete("Лучше используй AttributeInfo.")]
    public class AttributeRefInfo
    {
        public string Tag { get; set; }
        public string Text { get; set; }

        /// <summary>
        /// AttributeReference или AttributeDefinition
        /// </summary>
        public ObjectId IdAtrRef { get; set; }

        //public AttributeRefInfo(AttributeReference attr)
        //{
        //   Tag = attr.Tag;
        //   Text = attr.TextString;
        //   IdAtrRef = attr.Id;
        //}

        /// <summary>
        /// DBText - должен быть или AttributeDefinition или AttributeReference
        /// иначе исключение ArgumentException
        /// </summary>
        /// <param name="attr"></param>
        public AttributeRefInfo([NotNull] DBText attr)
        {
            if (attr is AttributeDefinition attdef)
            {
                Tag = attdef.Tag;
            }
            else
            {
                if (attr is AttributeReference attref)
                {
                    Tag = attref.Tag;
                }
                else
                {
                    throw new ArgumentException("requires an AttributeDefintion or AttributeReference");
                }
            }
            Text = attr.TextString;
            IdAtrRef = attr.Id;
        }

        [NotNull]
        public static List<AttributeRefInfo> GetAttrDefs(ObjectId idBtr)
        {
            var resVal = new List<AttributeRefInfo>();
            if (idBtr.IsNull) return resVal;
            using (var btr = idBtr.Open(OpenMode.ForRead) as BlockTableRecord)
            {
                foreach (var idEnt in btr)
                {
                    using (var attrDef = idEnt.Open(OpenMode.ForRead, false, true) as AttributeDefinition)
                    {
                        if (attrDef == null) continue;
                        var attrDefInfo = new AttributeRefInfo((DBText)attrDef);
                        resVal.Add(attrDefInfo);
                    }
                }
            }
            return resVal;
        }

        [NotNull]
        public static List<AttributeRefInfo> GetAttrRefs([CanBeNull] BlockReference blRef)
        {
            var resVal = new List<AttributeRefInfo>();
            if (blRef?.AttributeCollection != null)
            {
                foreach (ObjectId idAttrRef in blRef.AttributeCollection)
                {
                    using (var atrRef = idAttrRef.Open(OpenMode.ForRead, false, true) as AttributeReference)
                    {
                        var ai = new AttributeRefInfo(atrRef);
                        resVal.Add(ai);
                    }
                }
            }
            return resVal;
        }
    }
}
