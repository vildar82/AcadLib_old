namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;

    public class BoolVM : BaseValueVM
    {
        public static BoolView Create(IEnumerable<object> values,
            Action<object> update = null,
            Action<BoolVM> config = null,
            bool isReadOnly = false)
        {
            return Create<BoolView, BoolVM>(values, update, config, isReadOnly);
        }

        public static BoolView Create(
            object value,
            Action<object> update = null,
            Action<BoolVM> config = null,
            bool isReadOnly = false)
        {
            return Create<BoolView, BoolVM>(value, update, config, isReadOnly);
        }
    }
}
