using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace System
{
    [PublicAPI]
    [Obsolete]
    public static class StringExt
    {
        /// <summary>
        /// Удаление разделителей строк и др. \r\n?|\n
        /// </summary>
        /// <param name="input"></param>
        [NotNull]
        [Obsolete("Используй NetLib")]
        public static string ClearString([NotNull] this string input)
        {
            //return Regex.Replace(input, @"\r\n?|\n", "");
            return NetLib.StringExt.ClearString(input);
        }

        /// <summary>
        /// IndexOf(toCheck, comp) >= 0
        /// </summary>
        public static bool Contains([NotNull] this string source, [NotNull] string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Формат строки с аргументами string.Format();
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete]
        [NotNull]
#pragma warning disable IDE1006 // Naming Styles
        public static string f([NotNull] this string format, [NotNull] params object[] args)
#pragma warning restore IDE1006 // Naming Styles
        {
            return string.Format(format, args);
        }

        [NotNull]
        public static string GetValidDbSymbolName([NotNull] this string name)
        {
            //string testString = "<>/?\";:*|,='";
            var pattern = new Regex("[<>/?\";:*|,=']");
            var res = pattern.Replace(name, ".");
            res = res.Replace('\\', '.');
            SymbolUtilityServices.ValidateSymbolName(res, false);
            return res;
        }

        public static bool IsValidDbSymbolName(this string input)
        {
            try
            {
                SymbolUtilityServices.ValidateSymbolName(input, false);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}