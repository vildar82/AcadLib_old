namespace AcadLib.PaletteProps
{
    using System;
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
        public static Control CreateControl(this object value, Action<object> update)
        {
            switch (value)
            {
                case bool b: return BoolVM.Create(b, v => update(v));
                case int i: return IntVM.Create(i, v => update(v));
                case double d: return DoubleVM.Create(d, v => update(v));
                case string s: return StringVM.Create(s, v => update(v));
            }

            return null;
        }
    }
}
