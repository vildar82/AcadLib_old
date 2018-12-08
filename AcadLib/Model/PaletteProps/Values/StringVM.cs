namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;

    public class StringVM : BaseValueVM
    {
        public static StringView Create(IEnumerable<object> values,
            Action<object> update = null,
            Action<StringVM> config = null,
            bool isReadOnly = false)
        {
            return Create<StringView, StringVM>(values, update, config, isReadOnly);
        }

        public static StringView Create(
            object value,
            Action<object> update = null,
            Action<StringVM> config = null,
            bool isReadOnly = false)
        {
            return Create<StringView, StringVM>(value, update, config, isReadOnly);
        }
    }
}
