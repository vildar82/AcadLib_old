namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;

    public class IntVM : BaseValueVM<int?>
    {
        public int? Min { get; set; }
        public int? Max { get; set; }

        public static IntView Create(IEnumerable<int?> values,
            Action<int?> update = null,
            Action<IntVM> config = null,
            bool isReadOnly = false)
        {
            return Create<IntView, IntVM, int?>(values, update, config, isReadOnly);
        }

        public static IntView Create(
            int? value,
            Action<int?> update = null,
            Action<IntVM> config = null,
            bool isReadOnly = false,
            bool isVarious = false)
        {
            return Create<IntView, IntVM, int?>(value, update, config, isReadOnly, isVarious);
        }
    }
}