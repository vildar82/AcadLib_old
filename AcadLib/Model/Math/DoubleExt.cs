namespace AcadLib
{
    using System;
    using JetBrains.Annotations;

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