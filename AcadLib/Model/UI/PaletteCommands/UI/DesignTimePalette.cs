using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AcadLib.PaletteCommands.UI
{
    public class DesignTimePalette : PaletteModel
    {
        public DesignTimePalette()
        {
            var commands = new List<IPaletteCommand>();
            var block1 = new PaletteInsertBlock("1", @"c:\temp\1.dwg", "Блок 1", null, "Вставка блока 1");
            commands.Add(block1);

            PaletteCommands = new ObservableCollection<IPaletteCommand>(commands);            
        }
    }
}
