using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;
using MicroMvvm;
using System.ComponentModel;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace TestAcadlib.WPF
{
    public class Commands
    {
        [CommandMethod("TestWpf")]
        public void TestWpf()
        {
            var model = new Class1 {IntTextBox = 5};
            var w = new Window1(model);
            Application.ShowModalWindow(w);
        }
    }

    public class Class1
    {
        public MyEnum Test { get; set; }
        public Color Color { get; set; } = Color.FromColor(System.Drawing.Color.Aquamarine);
        public int Size { get; set; }
        public int IntTextBox { get; set; }
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
