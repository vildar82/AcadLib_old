using System;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AcadLib
{
    [PublicAPI]
    public static class DoubleExt
    {
        [Obsolete("Используй NetLib", true)]
        public static double Round(this double value, int digits = 4)
        {
            return Math.Round(value, digits);
        }
    }
}