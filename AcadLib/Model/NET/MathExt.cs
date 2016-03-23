using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib
{
    public static class MathExt
    {
        /// <summary>
        /// Преобразование градусов в радианы (Math.PI / 180.0)*angleDegrees
        /// </summary>
        /// <param name="angleDegrees">Угол в градусах</param>
        /// <returns>Угол в радианах</returns>
        public static double ToRadians(this double angleDegrees)
        {
            return angleDegrees * (Math.PI / 180.0);
        }

        public static int RoundTo10(int i)
        {
            if (i % 10 != 0)
            {
                i = ((i + 5) / 10) * 10;
            }
            return i;
        }

        public static int RoundTo100(int i)
        {
            if (i % 100 != 0)
            {
                i = ((i + 50) / 100) * 100;
            }
            return i;
        }

        /// <summary>
        /// Список чисел в строку, с групперовкой последовательных номеров
        /// ints = 1, 2, 3, 4, 5, 7, 8, 10, 15, 16, 100, 101, 102, 103, 105, 106, 107, 109
        /// res = "1-8,10,15,16,100-107,109"
        /// </summary>
        /// <param name="ints"></param>
        /// <returns></returns>
        public static string IntsToStringSequence(int[] ints)
        {
            // int[] paleNumbersInt = new[] { 1, 2, 3, 4, 5, 7, 8, 10, 15, 16, 100, 101, 102, 103, 105, 106, 107, 109 };
            // res = 1-8,10,15,16,100-107,109
            bool isFirstEnter = true;
            bool isWas = false;
            string mark = string.Empty;
            for (int i = 0; i < ints.Length; i++)
            {
                if (i == ints.Length - 1)
                {
                    if (mark.Length == 0)
                    {
                        mark += ints[i];
                        break;
                    }
                    if (mark[mark.Length - 1] != '-') mark += ",";
                    mark += ints[i];
                    break;

                }
                if ((i == 0) || (isFirstEnter))
                {
                    mark += ints[i].ToString();
                    isFirstEnter = false;
                    continue;
                }
                if (ints[i + 1] - ints[i] == 1)
                {
                    if (mark[mark.Length - 1] != '-') mark += "-";
                    isWas = true;
                    continue;
                }
                else
                {
                    if (mark[mark.Length - 1] != '-') mark += ",";
                    if (!isWas) mark += ints[i].ToString() + ",";
                    else
                    {
                        isWas = false;
                        mark += ints[i].ToString() + ",";
                    }

                    isFirstEnter = true;
                }
            }
            mark = mark.Replace(",,", ",");
            return mark;
        }        
    }
}
