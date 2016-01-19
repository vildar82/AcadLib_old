using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib
{
   public static class MathExt
   {
      /// <summary>
      /// Преобразование градусов в радианы (Math.PI / 180.0)*angleDegrees
      /// </summary>
      /// <param name="angleDegrees">Угол в градусах</param>
      /// <returns>Угол в радианах</returns>
      public static double ToRadians(this double angleDegrees)
      {
         return angleDegrees * (Math.PI / 180.0);
      }
   }
}
