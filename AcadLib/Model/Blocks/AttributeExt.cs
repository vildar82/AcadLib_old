using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using AcadLib.Blocks;

namespace AcadLib.Extensions
{
    /// <summary>
    /// Расширенные методы AttributeReference
    /// </summary>
    public static class AttributeExt
    {
        /// <summary>
        /// Поворот атрибута в 0
        /// </summary>        
        public static void Normalize(this AttributeReference atr)
        {
            if (atr.Rotation != 0)
            {
                if (!atr.IsWriteEnabled)
                    atr.UpgradeOpen();
                atr.Rotation = 0;
            }
        }

        public static bool Is(this AttributeReference attr, string tag)
        {
            return string.Equals(attr.Tag, tag, StringComparison.CurrentCultureIgnoreCase);
        }

        public static IEnumerable<AttributeInfo> EnumerateAttributes(this BlockReference blRef)
        {
            if (blRef == null) yield break;

            if (blRef.AttributeCollection != null)
            {
                foreach (ObjectId idAtr in blRef.AttributeCollection)
                {
                    if (!idAtr.IsValidEx()) continue;
                    var atr = idAtr.GetObject(OpenMode.ForRead) as AttributeReference;
                    if (atr == null) continue;
                    yield return new AttributeInfo(atr);
                }
            }
            var btr = (BlockTableRecord)blRef.BlockTableRecord.GetObject(OpenMode.ForRead);
            if (btr.HasAttributeDefinitions)
            {
                foreach (var id in btr)
                {
                    if (!id.IsValidEx()) continue;
                    var attdef = id.GetObject(OpenMode.ForRead) as AttributeDefinition;
                    if (attdef == null) continue;
                    if (attdef.Constant)
                        yield return new AttributeInfo(attdef);
                }
            }
        }

        /// <summary>
        /// Requires a transaction (not an OpenCloseTransaction) to be active when called:
        /// Returns an enumeration of all AttributeDefinitions whose Constant property is
        /// true, and all AttributeReferences attached to the block reference.
        /// </summary>      
        public static IEnumerable<DBText> GetAttributes(this BlockReference blockRef)
        {
            var tr = blockRef.GetTransaction();
            var btr = (BlockTableRecord)blockRef.BlockTableRecord.GetObject(OpenMode.ForRead);
            if (blockRef.AttributeCollection != null)
            {
                foreach (ObjectId id in blockRef.AttributeCollection)
                {
                    if (id.IsValidEx()) continue;
                    yield return (AttributeReference)tr.GetObject(id, OpenMode.ForRead);
                }
            }
            if (btr.HasAttributeDefinitions)
            {
                foreach (var id in btr)
                {
                    if (!id.IsValidEx()) continue;
                    var attdef = tr.GetObject(id, OpenMode.ForRead) as AttributeDefinition;
                    if (attdef == null) continue;
                    if (attdef.Constant)
                        yield return attdef;
                }
            }
        }

        // Requires an active transaction (not an OpenCloseTransaction)
        // Returns a dictionary whose values are either constant AttributeDefinitions
        // or AttributeReferences, keyed to their tags:

        public static Dictionary<string, DBText> GetAttributeDictionary(this BlockReference blockref)
        {
            return blockref.GetAttributes().Where(a => a.Visible).ToDictionary(a => GetTag(a), StringComparer.OrdinalIgnoreCase);
        }

        public static Transaction GetTransaction(this DBObject obj)
        {
            if (obj.Database == null)
                throw new ArgumentException("No database");
            var tr = obj.Database.TransactionManager.TopTransaction;
            if (tr == null)
                throw new InvalidOperationException("No active transaction");
            return tr;
        }

        static string GetTag(DBText dbtext)
        {
            var attdef = dbtext as AttributeDefinition;
            if (attdef != null)
                return attdef.Tag;
            var attref = dbtext as AttributeReference;
            if (attref != null)
                return attref.Tag;
            throw new ArgumentException("requires an AttributeDefintion or AttributeReference");
        }
    }
}
