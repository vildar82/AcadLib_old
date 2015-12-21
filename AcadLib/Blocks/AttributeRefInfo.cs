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
   /// </summary>
   public class AttributeRefInfo
   {
      private string _tag;
      private string _text;
      private ObjectId _idAtrRef;      

      public AttributeRefInfo(AttributeReference attr)
      {
         _tag = attr.Tag;
         _text = attr.TextString;
         _idAtrRef = attr.Id;
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
            _tag = attdef.Tag;
         }
         else
         {
            AttributeReference attref = attr as AttributeReference;
            if (attref != null)
            {
               _tag = attref.Tag;
            }
            else
            {
               throw new ArgumentException("requires an AttributeDefintion or AttributeReference");
            }
         }
         _text = attr.TextString;
         _idAtrRef = attr.Id;
      }

      public string Tag { get { return _tag; } }
      public string Text { get { return _text; } }
      public ObjectId IdAtrRef { get { return _idAtrRef; } }      
   }
}
