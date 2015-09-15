using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadLib.Blocks
{
   public class BlockAttribute
   {
      private string _name;
      private Dictionary<string, string> _atts;

      // Public read only properties
      public string Name
      {
         get { return _name; }
      }

      public Dictionary<string, string> Attributes
      {
         get { return _atts; }
      }

      public string this[string key]
      {
         get { return _atts[key.ToUpper()]; }
      }

      // Constructors
      public BlockAttribute(BlockReference br)
      {
         SetProperties(br);
      }

      public BlockAttribute(ObjectId id)
      {
         Document doc = AcAp.DocumentManager.MdiActiveDocument;
         using (Transaction tr = doc.TransactionManager.StartTransaction())
         {
            SetProperties(tr.GetObject(id, OpenMode.ForRead) as BlockReference);
         }
      }

      // Public method
      new public string ToString()
      {
         if (_atts != null && _atts.Count > 0)
            return string.Format("{0}: {1}",
                _name,
                _atts.Select(a => string.Format("{0}={1}", a.Key, a.Value))
                    .Aggregate((a, b) => string.Format("{0}; {1}", a, b)));
         return _name;
      }

      // Private method
      private void SetProperties(BlockReference br)
      {
         if (br == null) return;
         _name = br.GetEffectiveName();
         _atts = new Dictionary<string, string>();
         br.AttributeCollection
             .GetObjects<AttributeReference>()
             .Iterate(att => _atts.Add(att.Tag.ToUpper(), att.TextString));
      }
   }

   public class BlockAttributeEqualityComparer : IEqualityComparer<BlockAttribute>
   {
      public bool Equals(BlockAttribute x, BlockAttribute y)
      {
         return
             x.Name.Equals(y.Name, StringComparison.CurrentCultureIgnoreCase) &&
             x.Attributes.SequenceEqual(y.Attributes);
      }

      public int GetHashCode(BlockAttribute obj)
      {
         return base.GetHashCode();
      }
   }
}