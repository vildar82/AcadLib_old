namespace AcadLib.UI.Properties
{
    using System;
    using JetBrains.Annotations;
    using NetLib;
    using NetLib.WPF;
    using ReactiveUI;

    public class PropertiesViewModel : BaseViewModel
    {
        public PropertiesViewModel(object value, [CanBeNull] Func<object, object> reset = null)
        {
            Value = value;
            OK = CreateCommand(() => DialogResult = true);
            Reset = CreateCommand(() =>
            {
                if (reset != null)
                    Value = reset(value);
            });
        }

        [Reactive]
        public object Value { get; set; }

        public ReactiveCommand OK { get; set; }

        public ReactiveCommand Reset { get; set; }
    }

    public class DesignPropertiesViewModel : PropertiesViewModel
    {
        public DesignPropertiesViewModel() : base(null)
        {
        }
    }
}