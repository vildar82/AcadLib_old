namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using NetLib;

    public class IntVM : BaseValueVM
    {
        public static IntView Create(IEnumerable<object> values,
            Action<object> update = null,
            Action<IntVM> config = null,
            bool isReadOnly = false)
        {
            var updateA = GetUpdateAction(update);
            return Create<IntView, IntVM>(values, updateA, config, isReadOnly);
        }

        public static IntView Create(
            object value,
            Action<object> update = null,
            Action<IntVM> config = null,
            bool isReadOnly = false)
        {
            var updateA = GetUpdateAction(update);
            return Create<IntView, IntVM>(value, updateA, config, isReadOnly);
        }

        private static Action<object> GetUpdateAction(Action<object> update)
        {
            if (update == null)
                return null;
            return v => { update(v.GetValue<int>()); };
        }
    }
}
