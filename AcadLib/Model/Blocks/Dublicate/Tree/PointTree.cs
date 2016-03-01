using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.Blocks.Dublicate.Tree
{
   public struct PointTree : IEquatable<PointTree>
   {
      public static int tolerance = 10;
      public int X;
      public int Y;
      private int hX;
      private int hY;

      public PointTree(double x, double y)
      {
         X = Convert.ToInt32(x);
         Y = Convert.ToInt32(y);
         hX = MathExt.RoundTo100(X);
         hY = MathExt.RoundTo100(Y);
      }         

      public bool Equals(PointTree other)
      {
         return Math.Abs(X - other.X) < tolerance &&
                Math.Abs(Y - other.Y) < tolerance;
      }

      public override int GetHashCode()
      {
         return hX ^ hY;
      }
   }
}
