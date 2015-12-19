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
      public static bool Is (this AttributeReference attr, string tag)
      {
         return string.Equals(attr.TextString, tag, StringComparison.CurrentCultureIgnoreCase);
      }
   }
}
