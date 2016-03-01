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

      public static int RoundTo10(int i)
      {
         if (i % 10 != 0)
         {
            i = ((i + 5) / 10) * 10;
         }
         return i;
      }

      public static int RoundTo100(int i)
      {
         if (i % 100 != 0)
         {
            i = ((i + 50) / 100) * 100;
         }
         return i;
      }
   }
}
