using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Area area = Area.Zero;
            if (source == null)
            {
                return Area.Zero;
            }
            foreach (Area current in source)
            {
                area += current;
            }
            return area;
        }
    }
}
