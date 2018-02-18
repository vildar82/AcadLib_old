using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Threading;
using JetBrains.Annotations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

// ReSharper disable once CheckNamespace
namespace AcadLib.PaletteCommands
{
    public class SplitCommand : PaletteCommand
    {
        public List<PaletteCommand> Commands { get; set; }

        [Reactive] public PaletteCommand SelectedCommand { get; set; }

        public double ImageSize { get; set; }

        public SplitCommand([NotNull] List<PaletteCommand> commands)
        {
            Commands = commands;
            var c = commands[0];
            SelectedCommand = c;
            Access = c.Access;
            CommandName = c.CommandName;
            Description = c.Description;
            Group = c.Group;
            Image = c.Image;
            Name = c.Name;
            Index = c.Index;
            this.WhenAnyValue(v => v.SelectedCommand).Skip(1).ObserveOn(Dispatcher.CurrentDispatcher)
                .Subscribe(s => SelectedCommand.Execute());
            ImageSize = Properties.Settings.Default.PaletteImageSize * 2;
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        private void Default_PropertyChanged(object sender, [NotNull] System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.PaletteImageSize))
            {
                ImageSize = Properties.Settings.Default.PaletteImageSize * 2;
            }
        }
    }
}
