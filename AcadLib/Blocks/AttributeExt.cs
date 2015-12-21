using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Extensions
{
   /// <summary>
   /// Расширенные методы AttributeReference
   /// </summary>
   public static class AttributeExt
   {
      public static bool Is(this AttributeReference attr, string tag)
      {
         return string.Equals(attr.Tag, tag, StringComparison.CurrentCultureIgnoreCase);
      }

      // Requires a transaction (not an OpenCloseTransaction) to be active when called:
      // Returns an enumeration of all AttributeDefinitions whose Constant property is
      // true, and all AttributeReferences attached to the block reference.

      public static IEnumerable<DBText> GetAttributes(this BlockReference blockRef)
      {
         Transaction tr = blockRef.GetTransaction();
         BlockTableRecord btr = (BlockTableRecord)blockRef.DynamicBlockTableRecord.GetObject(OpenMode.ForRead);
         if (btr.HasAttributeDefinitions)
         {
            foreach (ObjectId id in blockRef.AttributeCollection)
            {
               yield return (AttributeReference)tr.GetObject(id, OpenMode.ForRead);
            }
            foreach (ObjectId id in btr)
            {
               if (id.ObjectClass.Name == "AcDbAttributeDefinition")
               {
                  AttributeDefinition attdef = (AttributeDefinition)tr.GetObject(id, OpenMode.ForRead);
                  if (attdef.Constant)
                     yield return attdef;
               }
            }
         }
      }

      // Requires an active transaction (not an OpenCloseTransaction)
      // Returns a dictionary whose values are either constant AttributeDefinitions
      // or AttributeReferences, keyed to their tags:

      public static Dictionary<string, DBText> GetAttributeDictionary(this BlockReference blockref)
      {
         return blockref.GetAttributes().ToDictionary(a => GetTag(a), StringComparer.OrdinalIgnoreCase);
      }

      public static Transaction GetTransaction(this DBObject obj)
      {
         if (obj.Database == null)
            throw new ArgumentException("No database");
         Transaction tr = obj.Database.TransactionManager.TopTransaction;
         if (tr == null)
            throw new InvalidOperationException("No active transaction");
         return tr;
      }

      static string GetTag(DBText dbtext)
      {
         AttributeDefinition attdef = dbtext as AttributeDefinition;
         if (attdef != null)
            return attdef.Tag;
         AttributeReference attref = dbtext as AttributeReference;
         if (attref != null)
            return attref.Tag;
         throw new ArgumentException("requires an AttributeDefintion or AttributeReference");
      }
   }
}


