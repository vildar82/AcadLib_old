namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;

    public class IntListVM : IntVM
    {
        public bool AllowCustomValue { get; set; }

        public List<object> Values { get; set; }

        public static IntListView Create(IEnumerable<object> values,
            Action<object> update = null,
            Action<IntListVM> config = null,
            bool isReadOnly = false)
        {
            return Create<IntListView, IntListVM>(values, update, config, isReadOnly);
        }

        public static IntListView Create(
            object value,
            Action<object> update = null,
            Action<IntListVM> config = null,
            bool isReadOnly = false)
        {
            return Create<IntListView, IntListVM>(value, update, config, isReadOnly);
        }
    }
}
