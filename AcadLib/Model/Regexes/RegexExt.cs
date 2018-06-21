namespace AcadLib.Regexes
{
    using System;
    using System.Text.RegularExpressions;
    using JetBrains.Annotations;

    [Obsolete]
    [PublicAPI]
    public static class RegexExt
    {
        /// <summary>
        /// Определение числа из строки начинающегося с целого числа
        /// 0 - если число не определено
        /// </summary>
        public static int StartInt([NotNull] this string input)
        {
            var value = 0;
            var resRegex = Regex.Match(input, @"^\d+");
            if (resRegex.Success)
            {
                int.TryParse(resRegex.Value, out value);
            }

            return value;
        }
    }
}