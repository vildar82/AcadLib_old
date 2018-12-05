namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// Методы расширения для свойтв на палитре
    /// </summary>
    public static class ValuesExt
    {
        /// <summary>
        /// Создать дефолтный контрол по типу свойства - bool, int, double, string
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="update">Обновление значения</param>
        /// <returns>Контрол для палитры</returns>
        public static Control CreateControl(this object value, Action<object> update, bool isReadOnly = false)
        {
            switch (value)
            {
                case bool b: return BoolVM.Create(b, v => update(v), isReadOnly: isReadOnly);
                case int i: return IntVM.Create(i, v => update(v), isReadOnly: isReadOnly);
                case double d: return DoubleVM.Create(d, v => update(v), isReadOnly: isReadOnly);
                case string s: return StringVM.Create(s, v => update(v), isReadOnly: isReadOnly);
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
            var value = GetValue(values);
            return CreateControl(value, update, isReadOnly);
        }

        private static object GetValue(IEnumerable<object> values)
        {
            var uniqValues = values.GroupBy(g => g).Select(s => s.Key);
            object value;
            if (uniqValues.Skip(1).Any())
            {
                return PalletePropsService.Various;
            }

            return uniqValues.FirstOrDefault();
        }
    }
}
