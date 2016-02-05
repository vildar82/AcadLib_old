using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Blocks
{
   /// <summary>
   /// Описание AttributeReference для хранения
   /// Так же подходит для AttributeDefinition
   /// </summary>
   public class AttributeRefInfo
   {
      public string Tag { get; set; }
      public string Text { get; set; }
      public ObjectId IdAtrRef { get; set; }

      public AttributeRefInfo(AttributeReference attr)
      {
         Tag = attr.Tag;
         Text = attr.TextString;
         IdAtrRef = attr.Id;
      }

      /// <summary>
      /// DBText - должен быть или AttributeDefinition или AttributeReference
      /// иначе исключение ArgumentException
      /// </summary>
      /// <param name="attr"></param>
      public AttributeRefInfo(DBText attr)
      {
         AttributeDefinition attdef = attr as AttributeDefinition;
         if (attdef != null)
         {
            Tag = attdef.Tag;
         }
         else
         {
            AttributeReference attref = attr as AttributeReference;
            if (attref != null)
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

      public static List<AttributeRefInfo> GetAttrDefs(ObjectId idBtr)
      {
         List<AttributeRefInfo> resVal = new List<AttributeRefInfo>();

         if (!idBtr.IsNull)
         {
            using (var btr = idBtr.Open(OpenMode.ForRead) as BlockTableRecord)
            {
               foreach (var idEnt in btr)
               {
                  using (var attrDef = idEnt.Open(OpenMode.ForRead, false, true) as AttributeDefinition)
                  {
                     if (attrDef != null)
                     {
                        var attrDefInfo = new AttributeRefInfo((DBText)attrDef);
                        resVal.Add(attrDefInfo);
                     }
                  }
               }
            }
         }
         return resVal;
      }
   }
}
