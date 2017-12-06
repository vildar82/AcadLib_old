using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExt
    {
        /// <summary>
        /// IndexOf(toCheck, comp) >= 0
        /// </summary>        
        public static bool Contains([NotNull] this string source, [NotNull] string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Удаление разделителей строк и др. \r\n?|\n
        /// </summary>
        /// <param name="input"></param>        
        [Obsolete("Используй NetLib")]
        public static string ClearString(this string input)
        {
            //return Regex.Replace(input, @"\r\n?|\n", "");
            return NetLib.StringExt.ClearString(input);
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

        [NotNull]
        public static string GetValidDbSymbolName([NotNull] this string name)
        {
            var res = name.ClearString();
            //string testString = "<>/?\";:*|,='";
            var pattern = new Regex("[<>/?\";:*|,=']");
            res = pattern.Replace(name, ".");
            res = res.Replace('\\', '.');
            SymbolUtilityServices.ValidateSymbolName(res, false);
            return res;
        }

        /// <summary>
        /// Формат строки с аргументами string.Format();
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [NotNull]
        public static string f([NotNull] this string format, [NotNull] params object[] args)
        {
            return string.Format(format, args);
        }
    }
}