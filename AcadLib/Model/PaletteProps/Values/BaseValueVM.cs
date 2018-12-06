namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Controls;
    using NetLib.WPF;
    using ReactiveUI;

    public abstract class BaseValueVM<T> : BaseModel, IValue
    {
        public bool IsReadOnly { get; set; }

        public bool IsVarious { get; set; }

        public T Value { get; set; }

        public static TView Create<TView, TVm, TValue>(
            IEnumerable<TValue> values,
            Action<TValue> update = null,
            Action<TVm> configure = null,
            bool isReadOnly = false)
            where TVm : BaseValueVM<TValue>, new()
        {
            var uniqValues = values.GroupBy(g => g).Select(s => s.Key);
            TValue value;
            var isVarious = false;
            if (uniqValues.Skip(1).Any())
            {
                isVarious = true;
                value = default;
            }
            else
            {
                value = uniqValues.FirstOrDefault();
            }

            return Create<TView, TVm, TValue>(value, v => Update(v, update), configure, isReadOnly, isVarious);
        }

        public static TView Create<TView, TVm, TValue>(
            TValue value,
            Action<TValue> update = null,
            Action<TVm> configure = null,
            bool isReadOnly = false,
            bool isVarious = false)
            where TVm : BaseValueVM<TValue>, new()
        {
            var vm = new TVm { Value = value, IsReadOnly = isReadOnly, IsVarious = isVarious };
            configure?.Invoke(vm);
            var valueObs = vm.WhenAnyValue(v => v.Value).Skip(1);
            valueObs.Subscribe(c => Update(c, update));
            if (isVarious)
                valueObs.Take(1).Subscribe(s => vm.IsVarious = false);
            return (TView)Activator.CreateInstance(typeof(TView), vm);
        }

        /// <inheritdoc />
        public void UpdateValue(object obj)
        {
            if (obj is T valT)
            {
                Value = valT;
            }
            else
            {
                AcadLib.Logger.Log.Warn($"IValue BaseValueVM UpdateValue - не тот тип объекта T={typeof(T).Name}, value={obj?.GetType().Name}/");
            }
        }

        protected static void Update<TValue>(TValue value, Action<TValue> update)
        {
            if (update == null)
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