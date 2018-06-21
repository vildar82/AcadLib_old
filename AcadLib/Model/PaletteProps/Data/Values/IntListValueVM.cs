namespace AcadLib.PaletteProps
{
    using System.Collections.Generic;

    public class IntListValueVM : IntValueVM
    {
        public bool AllowCustomValue { get; set; }

        public IEnumerable<int> Values { get; set; }
    }
}