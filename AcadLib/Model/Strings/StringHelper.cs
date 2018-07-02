namespace AcadLib.Strings
{
    using System;
    using System.Text.RegularExpressions;
    using JetBrains.Annotations;

    [Obsolete]
    public static class StringHelper
    {
        /// <summary>
        /// Определение числа из строки начинающейся числом.
        /// Например: "100 шт." = 100
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [NotNull]
        public static Result<int> GetStartInteger([CanBeNull] string input)
        {
            if (input != null)
            {
                var match = Regex.Match(input, @"^\d*");
                if (match.Success)
                {
                    if (int.TryParse(match.Value, out var value))
                    {
                        return Result.Ok(value);
                    }
                }
            }

            return Result.Fail<int>($"Не определено целое число из строки - {input}");
        }
    }
}