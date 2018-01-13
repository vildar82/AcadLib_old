using JetBrains.Annotations;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;

// ReSharper disable once CheckNamespace
namespace AcadLib.PaletteCommands
{
    public class PaletteModel : ReactiveObject
    {
        /// <summary>
        /// Цвет фона
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [Reactive] public System.Windows.Media.Brush Background { get; set; }

        /// <summary>
        /// Команды на палитре
        /// </summary>
        public ObservableCollection<IPaletteCommand> PaletteCommands { get; set; }

        public string Version { get; }

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

        protected PaletteModel()
        {
        }
    }
}