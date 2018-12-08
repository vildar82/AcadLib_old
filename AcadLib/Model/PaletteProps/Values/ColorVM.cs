namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Autodesk.AutoCAD.Colors;
    using ReactiveUI;

    public class ColorVM : BaseValueVM
    {
        public static ColorView Create(IEnumerable<object> values,
            Action<object> update = null,
            Action<ColorVM> config = null,
            bool isReadOnly = false)
        {
            return Create<ColorView, ColorVM>(values, update, config, isReadOnly);
        }

        public static ColorView Create(
            object value,
            Action<object> update = null,
            Action<ColorVM> config = null,
            bool isReadOnly = false)
        {
            return Create<ColorView, ColorVM>(value, update, config, isReadOnly);
        }
    }
}
