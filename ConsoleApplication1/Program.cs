using System;

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