using AcadLib.PaletteProps;
using Autodesk.AutoCAD.Runtime;

namespace TestAcadlib.PaletteProps
{
    public class TestPaletteProps
    {
        [CommandMethod(nameof(TestPalettePropsCom))]
        public void TestPalettePropsCom()
        {
            PalletePropsService.Registry();
        }
    }
}
