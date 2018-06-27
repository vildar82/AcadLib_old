namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Autodesk.AutoCAD.Colors;
    using ReactiveUI;

    public class ColorVM : BaseValueVM<Color>
    {
        public static ColorView Create(IEnumerable<Color> values,
            Action<Color> update,
            Action<ColorVM> config = null,
            bool isReadOnly = false)
        {
            return Create<ColorView, ColorVM, Color>(values, update, config, isReadOnly);
        }

        public static ColorView Create(
            Color value,
            Action<Color> update,
            Action<ColorVM> config = null,
            bool isReadOnly = false)
        {
            return Create<ColorView, ColorVM, Color>(value, update, config, isReadOnly);
        }
    }
}