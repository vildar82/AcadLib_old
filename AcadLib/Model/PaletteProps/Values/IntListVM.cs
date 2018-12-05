namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;

    public class IntListVM : IntVM
    {
        public bool AllowCustomValue { get; set; }

        public IEnumerable<int> Values { get; set; }

        public static IntListView Create(IEnumerable<int?> values,
            Action<int?> update = null,
            Action<IntListVM> config = null,
            bool isReadOnly = false)
        {
            return Create<IntListView, IntListVM, int?>(values, update, config, isReadOnly);
        }

        public static IntListView Create(
            int? value,
            Action<int?> update = null,
            Action<IntListVM> config = null,
            bool isReadOnly = false)
        {
            return Create<IntListView, IntListVM, int?>(value, update, config, isReadOnly);
        }
    }
}