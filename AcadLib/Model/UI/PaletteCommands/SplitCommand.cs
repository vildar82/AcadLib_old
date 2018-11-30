namespace AcadLib.PaletteCommands
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Threading;
    using JetBrains.Annotations;
    using NetLib;
    using ReactiveUI;

    public class SplitCommand : PaletteCommand
    {
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
            this.WhenAnyValue(v => v.SelectedCommand).Skip(1).ObserveOn(SynchronizationContext.Current)
                .Subscribe(s => SelectedCommand.Execute());
            ImageSize = Properties.Settings.Default.PaletteImageSize * 2;
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        public List<PaletteCommand> Commands { get; set; }

        [Reactive]
        public PaletteCommand SelectedCommand { get; set; }

        public double ImageSize { get; set; }

        private void Default_PropertyChanged(object sender, [NotNull] System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.PaletteImageSize))
            {
                ImageSize = Properties.Settings.Default.PaletteImageSize * 2;
            }
        }
    }
}