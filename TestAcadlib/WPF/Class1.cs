using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace TestAcadlib.WPF
{
    public class Commands
    {
        [CommandMethod("TestWpf")]
        public void Test ()
        {
            Window1 w = new WPF.Window1();
            Application.ShowModalWindow(w);
        }
    }

    public class Class1
    {
        public MyEnum Test { get; set; }
    }

    public enum MyEnum
    {
        [Description("Один")]
        One,
        [Description("Два")]
        Two,
        [Description("Три")]
        Three
    }
}
