namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using NetLib.WPF;
    using ReactiveUI;

    public abstract class BaseValueVM : BaseModel, IValue
    {
        public bool IsReadOnly { get; set; }

        public object Value { get; set; }

        public static TView Create<TView, TVm>(
            IEnumerable<object> values,
            Action<object> update = null,
            Action<TVm> configure = null,
            bool isReadOnly = false)
            where TVm : BaseValueVM, new()
        {
            var uniqValues = values.GroupBy(g => g).Select(s => s.Key);
            object value;
            var isVarious = false;
            value = uniqValues.Skip(1).Any() ? PalettePropsService.Various : uniqValues.FirstOrDefault();

            Action<object> updateVal = null;
            if (update != null)
                updateVal = v => Update(v, update);
            return Create<TView, TVm>(value, updateVal, configure, isReadOnly);
        }

        public static TView Create<TView, TVm>(
            object value,
            Action<object> update = null,
            Action<TVm> configure = null,
            bool isReadOnly = false)
            where TVm : BaseValueVM, new()
        {
            if (update == null)
                isReadOnly = true;
            var vm = new TVm { Value = value, IsReadOnly = isReadOnly};
            configure?.Invoke(vm);
            var valueObs = vm.WhenAnyValue(v => v.Value).Skip(1);
            valueObs.ObserveOnDispatcher()
                .Throttle(TimeSpan.FromMilliseconds(400))
                .Subscribe(c => Update(c, update));
            return (TView)Activator.CreateInstance(typeof(TView), vm);
        }

        /// <inheritdoc />
        public void UpdateValue(object obj)
        {
            Value = obj;
        }

        protected static void Update(object value, Action<object> update)
        {
            if (update == null || value == null || Equals(PalettePropsService.Various, value))
                return;
            var doc = AcadHelper.Doc;
            using (doc.LockDocument())
            using (var t = doc.TransactionManager.StartTransaction())
            {
                Debug.WriteLine($"Palette Props Update Value = {value}");
                update(value);
                t.Commit();
            }
        }
    }
}
