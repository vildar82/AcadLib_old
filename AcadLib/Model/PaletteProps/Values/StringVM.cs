namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;

    public class StringVM : BaseValueVM<string>
    {
        public static StringView Create(IEnumerable<string> values,
            Action<string> update = null,
            Action<StringVM> config = null,
            bool isReadOnly = false)
        {
            return Create<StringView, StringVM, string>(values, update, config, isReadOnly);
        }

        public static StringView Create(
            string value,
            Action<string> update = null,
            Action<StringVM> config = null,
            bool isReadOnly = false)
        {
            return Create<StringView, StringVM, string>(value, update, config, isReadOnly);
        }
    }
}