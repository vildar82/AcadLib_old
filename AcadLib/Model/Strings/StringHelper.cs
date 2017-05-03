using System.Text.RegularExpressions;

namespace AcadLib.Strings
{
    public static class StringHelper
    {
        /// <summary>
        /// Определение числа из строки начинающейся числом.
        /// Например: "100 шт." = 100
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Result<int> GetStartInteger (string input)
        {
            if (input != null)
            {
                var value = 0;
                var match = Regex.Match(input, @"^\d*");
                if (match.Success)
                {
                    if (int.TryParse(match.Value, out value))
                    {
                        return Result.Ok(value);
                    }
                }
            }
            return Result.Fail<int>($"Не определено целое число из строки - {input}");
        }
    }
}
