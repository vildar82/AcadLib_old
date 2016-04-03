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

        public static PaletteSetCommands Create(string name, List<IPaletteCommand> comms)
        {
            var palette = new PaletteSetCommands(name);
            PaletteModel model = new PaletteModel();
            model.LoadData(comms);
            CommandsControl commControl = new CommandsControl();
            commControl.DataContext = model;
            palette.AddVisual("Главная",commControl);
            return palette;
        }        
    }
}
