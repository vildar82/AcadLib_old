using System.Collections.Generic;

namespace AcadLib.PaletteProps
{
    public class IntListValueVM : BaseValueVM
    {
        public bool AllowCustomValue { get; set; }
        public List<int> Values { get; set; }
        public int Value { get; set; }
    }
}
