using System.Collections.Generic;

namespace AcadLib.Comparers
{
    /// <summary>
    /// Сравнение строк как чисел
    /// </summary>
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
                else
                {
                    // y - строка.
                    return -1; // число numberX меньше строки y
                }
            }
            else
            {
                // x - строка
                if (int.TryParse(y, out var numberY))
                {
                    // y - число numberY
                    return 1; // число numberY меньше строки x
                }
                else
                {
                    // y - строка.               
                    return x.CompareTo(y);
                }
            }
        }
    }
}
