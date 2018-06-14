using System.Collections.Generic;

namespace AcadLib.PaletteProps
{
    public class IntListValueVM : IntValueVM
    {
        public bool AllowCustomValue { get; set; }
        public IEnumerable<int> Values { get; set; }
    }
}
