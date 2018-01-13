using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AcadLib.Comparers
{
    /// <summary>
    /// Сравнение строк как чисел
    /// </summary>
    [PublicAPI]
    [Obsolete]
    public class StringsNumberComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (int.TryParse(x, out var numberX))
            {
                // x - число numberX
                if (int.TryParse(y, out var numberY))
                {
                    // y - число numberY
                    return numberX.CompareTo(numberY);
                }
                // y - строка.
                return -1; // число numberX меньше строки y
            }
            // x - строка
            return int.TryParse(y, out var _) ? 1 : string.CompareOrdinal(x, y);
            // y - строка.
        }
    }
}