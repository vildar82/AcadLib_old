using System;
using JetBrains.Annotations;

namespace AcadLib
{
    [PublicAPI]
    public static class DoubleExt
    {
        [Obsolete("Используй NetLib", true)]
        public static double Round(this double value, int digits = 4)
        {
            return NetLib.DoubleExt.Round(value, digits);
        }
    }
}