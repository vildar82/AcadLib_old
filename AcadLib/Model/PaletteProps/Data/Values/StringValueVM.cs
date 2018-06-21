namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using ReactiveUI;

    public class StringValueVM : BaseValueVM
    {
        public string Value { get; set; }

        public static StringValueView CreateValue(
            IEnumerable<string> values,
            Action<string> update,
            Action<StringValueVM> config = null,
            bool isReadOnly = false)
        {
            var uniqValues = values.GroupBy(g => g).Select(s => s.Key);
            var value = uniqValues.Skip(1).Any() ? null : uniqValues.FirstOrDefault();
            return CreateValue(value, update, config);
        }

        public static StringValueView CreateValue(
            string value,
            Action<string> update,
            Action<StringValueVM> config = null,
            bool isReadOnly = false)
        {
            var stringVM = new StringValueVM { Value = value, IsReadOnly = isReadOnly };
            config?.Invoke(stringVM);
            stringVM.WhenAnyValue(v => v.Value).Skip(1).Subscribe(update);
            return new StringValueView(stringVM);
        }
    }
}