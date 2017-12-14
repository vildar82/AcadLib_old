﻿using JetBrains.Annotations;
using NetLib.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace AcadLib.UI.Properties
{
    public class PropertiesViewModel : BaseViewModel
    {
        public PropertiesViewModel(object value, [CanBeNull] Func<object, object> reset = null)
        {
            Value = value;
            OK = ReactiveCommand.Create(() => DialogResult = true);
            Reset = AddCommand(ReactiveCommand.Create(() =>
            {
                if (reset != null) Value = reset(value);
            }));
        }

        [Reactive] public object Value { get; set; }
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