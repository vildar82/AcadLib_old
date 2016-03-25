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
            var ints = new[] { 1};
            info(ints);

            ints = new[] { 1, 2 };
            info(ints);

            ints = new[] { 1, 3, 5 };
            info(ints);

            ints = new[] { 1, 3, 5, 6, 7 };
            info(ints);

            ints = new[] { 1, 3, 5, 6, 7, 10 };
            info(ints);

            ints = new[] { 1, 3, 5, 6, 7, 10, 11,20,21,22,25,26 };
            info(ints);

            ints = new[] { 1, 3, 5, 6, 7, 10, 11, 20, 21, 22, 25, 26, 30,31 };
            info(ints);

            ints = new[] { 1, 3, 5, 6, 7, 10, 11, 20, 21, 22, 25, 26, 30, 31, 32 };
            info(ints);

            ints = new[] { 1, 2, 3, 10,11,12, 15,16, 25,26,27,28,29,30, 100 };
            info(ints);

            ints = new[] { 1, 2, 3, 10, 11, 12, 15, 16, 25, 26, 27, 28, 29, 30, 100 };
            info(ints);


            Console.ReadKey();
        }

        static void info(int [] ints)
        {
            Console.WriteLine(string.Join(",", ints));
            var res = AcadLib.MathExt.IntsToStringSequence(ints);
            Console.WriteLine(res);
        }
    }
}