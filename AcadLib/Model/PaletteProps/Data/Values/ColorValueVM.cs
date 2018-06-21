namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Autodesk.AutoCAD.Colors;
    using ReactiveUI;

    public class ColorValueVM : BaseValueVM
    {
        public Color Value { get; set; }

        public static ColorValueView CreateValue(
            IEnumerable<Color> colors,
            Action<Color> update,
            Action<ColorValueVM> configure = null,
            bool isReadOnly = false)
        {
            var uniqColors = colors.GroupBy(g => g).Select(s => s.Key);
            var color = uniqColors.Skip(1).Any() ? null : uniqColors.FirstOrDefault();
            return CreateValue(color, update, configure);
        }

        public static ColorValueView CreateValue(
            Color color,
            Action<Color> update,
            Action<ColorValueVM> configure = null,
            bool isReadOnly = false)
        {
            var vm = new ColorValueVM { Value = color, IsReadOnly = isReadOnly };
            configure?.Invoke(vm);
            vm.WhenAnyValue(v => v.Value).Skip(1).Subscribe(c => Update(c, update));
            return new ColorValueView(vm);
        }
    }
}