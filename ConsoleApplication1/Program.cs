using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks.Dublicate.Tree;
using System.Windows;

namespace ConsoleApplication1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {            
            Errors.TestErrors.TestShowErrors();
            Console.ReadKey();
        }        
    }
}