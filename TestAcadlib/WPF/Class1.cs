using System.ComponentModel;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace TestAcadlib.WPF
{
    public class Commands
    {
        [CommandMethod("TestWpf")]
        public void Test ()
        {
            var w = new Window1();
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
