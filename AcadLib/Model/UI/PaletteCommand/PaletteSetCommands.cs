using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.AutoCAD.Windows;

namespace AcadLib.PaletteCommands
{
    public class PaletteSetCommands :PaletteSet
    {
        public PaletteSetCommands(string name) : base(name)
        {                        
        }

        public PaletteSetCommands(string name, Guid guid) : base(name, guid)
        {
        }

        public PaletteSetCommands(string name, string cmd, Guid guid) : base(name, cmd, guid)
        {
        }

        public static PaletteSetCommands Create(string name, List<IPaletteCommand> comms, Guid guid)
        {
            var palette = new PaletteSetCommands(name, guid);
            PaletteModel model = new PaletteModel();
            model.LoadData(comms);
            CommandsControl commControl = new CommandsControl();
            commControl.DataContext = model;
            palette.AddVisual("Главная",commControl);
            return palette;
        }        
    }
}
