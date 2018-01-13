using JetBrains.Annotations;
using System;

// ReSharper disable once CheckNamespace
namespace AcadLib
{
    [PublicAPI]
    [Obsolete]
    public static class EnumExt
    {
        /// <summary>
        /// Конвертация строки в соответствующее значение перечисления enum
        /// Выбрасывает исключение при несоответствии
        /// </summary>
        [NotNull]
        public static T ToEnum<T>([NotNull] this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Конвертация строки в соответствующее значение перечисления enum.
        /// Ичключение не выбрасывапется. (если, только, T не структура)
        /// </summary>
        public static T ToEnum<T>([CanBeNull] this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return Enum.TryParse(value, true, out T result) ? result : defaultValue;
        }
    }
}