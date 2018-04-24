// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2016 04 03 16:09

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using AcadLib.Properties;
using AcadLib.UI.PaletteCommands.UI;
using JetBrains.Annotations;
using NetLib;
using NetLib.WPF;
using ReactiveUI;

// ReSharper disable once CheckNamespace
namespace AcadLib.PaletteCommands
{
    public class PaletteModel : BaseModel
    {
        public PaletteModel([NotNull] IEnumerable<IPaletteCommand> commands, string version)
        {
            Version = version;
            PaletteCommands = new ObservableCollection<IPaletteCommand>();
            foreach (var item in commands)
            {
                if (PaletteSetCommands.IsAccess(item.Access))
                    PaletteCommands.Add(item);
            }
            ChangeContent(Settings.Default.PaletteStyle);
        }

        protected PaletteModel()
        {
        }

        /// <summary>
        ///     Цвет фона
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [Reactive]
        public Brush Background { get; set; }

        [Reactive]
        public double ButtonWidth { get; set; } = PaletteSetCommands.GetButtonWidth();

        [Reactive]
        public UserControl Content { get; set; }

        [Reactive]
        public double FontMaxHeight { get; set; } = PaletteSetCommands.GetFontSize();

        /// <summary>
        ///     Команды на палитре
        /// </summary>
        public ObservableCollection<IPaletteCommand> PaletteCommands { get; set; }

        public string Version { get; }

        public void ChangeContent(int listStyle)
        {
            switch (listStyle)
            {
                case 1 when !(Content is ImagesAndText):
                    // Значки и текст
                    this.RaisePropertyChanged(nameof(ButtonWidth));
                    this.RaisePropertyChanged(nameof(FontMaxHeight));
                    Content = new ImagesAndText {DataContext = this};
                    return;
                case 2 when !(Content is ListCommands):
                    // Список
                    Content = new ListCommands {DataContext = this};
                    return;
                default:
                    // Только значки
                    if (!(Content is OnlyImages))
                        Content = new OnlyImages {DataContext = this};
                    break;
            }
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
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