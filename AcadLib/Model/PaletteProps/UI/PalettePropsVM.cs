using System.Collections.Generic;
using NetLib.WPF;

namespace AcadLib.PaletteProps.UI
{
    public class PalettePropsVM : BaseModel
    {
        public List<PalettePropsType> Types { get; set; }

        public PalettePropsType SelectedType { get; set; }

        public void Clear()
        {
            Types = null;
        }
    }
}
