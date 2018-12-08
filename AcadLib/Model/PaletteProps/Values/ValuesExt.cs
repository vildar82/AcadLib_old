using JetBrains.Annotations;

namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// Методы расширения для свойтв на палитре
    /// </summary>
    [PublicAPI]
    public static class ValuesExt
    {
        /// <summary>
        /// Создать дефолтный контрол по типу свойства - bool, int, double, string
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="update">Обновление значения</param>
        /// <returns>Контрол для палитры</returns>
        public static Control CreateControl(this object value, Action<object> update, bool isReadOnly = false, bool isVarious = false)
        {
            switch (value)
            {
                case bool b: return BoolVM.Create(b, v => update(v), isReadOnly: isReadOnly, isVarious: isVarious);
                case int i: return IntVM.Create(i, v => update(v), isReadOnly: isReadOnly, isVarious: isVarious);
                case double d: return DoubleVM.Create(d, v => update(v), isReadOnly: isReadOnly, isVarious: isVarious);
                case string s: return StringVM.Create(s, v => update(v), isReadOnly: isReadOnly, isVarious: isVarious);
            }

            return null;
        }

        /// <summary>
        /// Создать дефолтный контрол по типу свойства - bool, int, double, string
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="update">Обновление значения</param>
        /// <returns>Контрол для палитры</returns>
        public static Control CreateControl(this IEnumerable<object> values, Action<object> update, bool isReadOnly = false)
        {
            var value = GetValue(values, out var isVarious);
            return CreateControl(value, update, isReadOnly, isVarious);
        }

        private static object GetValue(IEnumerable<object> values, out bool isVarious)
        {
            var uniqValues = values.GroupBy(g => g).Select(s => s.Key);
            object value;
            if (uniqValues.Skip(1).Any())
            {
                isVarious = true;
                return string.Empty;
            }

            isVarious = false;
            return uniqValues.FirstOrDefault();
        }
    }
}
