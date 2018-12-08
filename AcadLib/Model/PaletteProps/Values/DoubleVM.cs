namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;

    public class DoubleVM : BaseValueVM
    {
        public static DoubleView Create(IEnumerable<object> values,
            Action<object> update = null,
            Action<DoubleVM> config = null,
            bool isReadOnly = false)
        {
            var updateA = GetUpdateAction(update);
            return Create<DoubleView, DoubleVM>(values, updateA, config, isReadOnly);
        }

        public static DoubleView Create(
            object value,
            Action<object> update = null,
            Action<DoubleVM> config = null,
            bool isReadOnly = false)
        {
            var updateA = GetUpdateAction(update);
            return Create<DoubleView, DoubleVM>(value, updateA, config, isReadOnly);
        }

        private static Action<object> GetUpdateAction(Action<object> update)
        {
            throw new NotImplementedException();
        }
    }
}
