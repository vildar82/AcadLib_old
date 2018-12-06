namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;

    public class BoolVM : BaseValueVM<bool?>
    {
        public static BoolView Create(IEnumerable<bool?> values,
            Action<bool?> update = null,
            Action<BoolVM> config = null,
            bool isReadOnly = false)
        {
            return Create<BoolView, BoolVM, bool?>(values, update, config, isReadOnly);
        }

        public static BoolView Create(
            bool? value,
            Action<bool?> update = null,
            Action<BoolVM> config = null,
            bool isReadOnly = false,
            bool isVarious = false)
        {
            return Create<BoolView, BoolVM, bool?>(value, update, config, isReadOnly, isVarious);
        }
    }
}
