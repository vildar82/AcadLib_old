using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace AcadLib.Units
{
    public static class UnitsNetExt
    {        
        public static Area Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Area> selector)
        {
            return source.Select(selector).Sum();
        }

        public static Area Sum(this IEnumerable<Area> source)
        {
            var area = Area.Zero;
            if (source != null)
            {
                foreach (var current in source)
                {
                    area += current;
                }
            }
            return area;
        }

        public static Length Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Length> selector)
        {
            return source.Select(selector).Sum();
        }

        public static Length Sum(this IEnumerable<Length> source)
        {
            var length = Length.Zero;
            if (source != null)
            {
                foreach (var current in source)
                {
                    length += current;
                }
            }
            return length;
        }
    }
}
