using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;

namespace AcadLib.Blocks
{
    public static class AttSyncExt
    {
	    

        public static void SynchronizeAttributes(this ObjectId btrId)
        {
            using (var t = btrId.Database.TransactionManager.StartTransaction())
            {
                var btr = btrId.GetObject(OpenMode.ForRead) as BlockTableRecord;
                btr.SynchronizeAttributes();
                t.Commit();
            }
        }

        /// <summary>
        /// Синхронизация атрибутов блока. Требуется запущенная транзакция
        /// </summary>        
        public static void SynchronizeAttributes(this BlockTableRecord target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var tr = target.Database.TransactionManager.TopTransaction;
            if (tr == null)
                throw new Autodesk.AutoCAD.Runtime.Exception(ErrorStatus.NoActiveTransactions);
            List<AttributeDefinition> attDefs = target.GetAttributes(tr);
            foreach (ObjectId id in target.GetBlockReferenceIds(true, false))
            {
                var br = (BlockReference)tr.GetObject(id, OpenMode.ForWrite);
                br.ResetAttributes(attDefs, tr);
            }
            if (target.IsDynamicBlock)
            {
                target.UpdateAnonymousBlocks();
                foreach (ObjectId id in target.GetAnonymousBlockIds())
                {
                    var btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);
                    attDefs = btr.GetAttributes(tr);
                    foreach (ObjectId brId in btr.GetBlockReferenceIds(true, false))
                    {
                        var br = (BlockReference)tr.GetObject(brId, OpenMode.ForWrite);
                        br.ResetAttributes(attDefs, tr);
                    }
                }
            }
        }

        private static List<AttributeDefinition> GetAttributes(this BlockTableRecord target, Transaction tr)
        {
            var attDefs = new List<AttributeDefinition>();
            foreach (var id in target)
            {
                if (id.ObjectClass == General.ClassAttDef)
                {
                    var attDef = (AttributeDefinition)tr.GetObject(id, OpenMode.ForRead);
                    attDefs.Add(attDef);
                }
            }
            return attDefs;
        }

        private static void ResetAttributes(this BlockReference br, List<AttributeDefinition> attDefs, Transaction tr)
        {
            var attValues = new Dictionary<string, string>();
            foreach (ObjectId id in br.AttributeCollection)
            {
                if (!id.IsErased)
                {
                    var attRef = tr.GetObject(id, OpenMode.ForWrite) as AttributeReference;
                    if (attRef == null) continue;
                    attValues.Add(attRef.Tag,
                        attRef.IsMTextAttribute ? attRef.MTextAttribute.Contents : attRef.TextString);
                    attRef.Erase();
                }
            }
            foreach (var attDef in attDefs)
            {
                var attRef = new AttributeReference();
                attRef.SetAttributeFromBlock(attDef, br.BlockTransform);
                if (attDef.Constant)
                {
                    attRef.TextString = attDef.IsMTextAttributeDefinition ?
                        attDef.MTextAttributeDefinition.Contents :
                        attDef.TextString;
                }
                else if (attValues.ContainsKey(attRef.Tag))
                {
                    attRef.TextString = attValues[attRef.Tag];
                }
                br.AttributeCollection.AppendAttribute(attRef);
                tr.AddNewlyCreatedDBObject(attRef, true);
            }
        }
    }
}