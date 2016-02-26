using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks.Dublicate.Tree;

namespace ConsoleApplication1
{
   class Program
   {
      static void Main(string[] args)
      {
         PointTree ptTree = new PointTree(45997.554, 0);
         PointTree ptTree2 = new PointTree(46000.554, 0);
         var res = AcadLib.MathExt.RoundTo100(45999);

         Console.WriteLine(res);

         Console.ReadKey();
      }
   }
}
