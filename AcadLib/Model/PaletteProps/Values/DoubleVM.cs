namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;

    public class DoubleVM : BaseValueVM<double?>
    {
        public double? Min { get; set; }
        public double? Max { get; set; }

        public static DoubleView Create(IEnumerable<double?> values,
            Action<double?> update = null,
            Action<DoubleVM> config = null,
            bool isReadOnly = false)
        {
            return Create<DoubleView, DoubleVM, double?>(values, update, config, isReadOnly);
        }

        public static DoubleView Create(
            double? value,
            Action<double?> update = null,
            Action<DoubleVM> config = null,
            bool isReadOnly = false)
        {
            return Create<DoubleView, DoubleVM, double?>(value, update, config, isReadOnly);
        }
    }
}