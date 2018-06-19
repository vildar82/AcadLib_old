using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Colors;
using NetLib.WPF;
using ReactiveUI;

namespace AcadLib.PaletteProps
{
    public class ColorValueVM : BaseValueVM
    {
        public Color Value { get; set; }

        public static ColorValueView CreateValue(Color color, Action<Color> update, Action<ColorValueVM> configure = null) 
        {
            var vm = new ColorValueVM { Value = color };
            configure?.Invoke(vm);
            vm.WhenAnyValue(v => v.Value).Subscribe(c=> Update(c, update));
            return new ColorValueView(vm);
        }

        private static void Update(Color color, Action<Color> update)
        {
            var doc = AcadHelper.Doc;
            using (doc.LockDocument())
            using (var t = doc.TransactionManager.StartTransaction())
            {
                update(color);
                t.Commit();
            }
        }
    }
}
