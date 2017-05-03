using System;

namespace AcadLib
{
    public static class EnumExt
    {
        /// <summary>
        /// Конвертация строки в соответствующее значение перечисления enum
        /// Выбрасывает исключение при несоответствии
        /// </summary>        
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Конвертация строки в соответствующее значение перечисления enum.
        /// Ичключение не выбрасывапется. (если, только, T не структура)
        /// </summary>        
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            T result;
            return Enum.TryParse(value, true, out result) ? result : defaultValue;
        }
    }
}
