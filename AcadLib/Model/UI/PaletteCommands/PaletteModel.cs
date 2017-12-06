using JetBrains.Annotations;
using NetLib.WPF;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace AcadLib.PaletteCommands
{
    public class PaletteModel : ReactiveObject
    {
        protected PaletteModel()
        {

        }

        public PaletteModel([NotNull] IEnumerable<IPaletteCommand> commands, string version)
        {
            Version = version;
            PaletteCommands = new ObservableCollection<IPaletteCommand>();
            foreach (var item in commands)
            {
                if (PaletteSetCommands.IsAccess(item.Access))
                {
                    PaletteCommands.Add(item);
                }
            }
        }

        /// <summary>
        /// Цвет фона
        /// </summary>
        [Reactive] public System.Windows.Media.Brush Background { get; set; }
        /// <summary>
        /// Команды на палитре
        /// </summary>
        public ObservableCollection<IPaletteCommand> PaletteCommands { get; set; }

        public string Version { get; }
    }
}
