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
      public double X;
      public double Y;

      public PointTree(double x, double y)
      {
         X = x;
         Y = y;
      }         

      public bool Equals(PointTree other)
      {
         return Math.Abs(X - other.X) < tolerance &&
                Math.Abs(Y - other.Y) < tolerance;
      }
   }
}
