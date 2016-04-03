using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.PaletteCommands
{
    public class PaletteModel
    {
        public ObservableCollection<IPaletteCommand> PaletteCommands { get; private set; }

        public PaletteModel()
        {
            PaletteCommands = new ObservableCollection<IPaletteCommand>();            
        }

        public void LoadData(List<IPaletteCommand> paletteCommands)
        {
            foreach (var item in paletteCommands)
            {
                PaletteCommands.Add(item);
            }
        }
    }
}
