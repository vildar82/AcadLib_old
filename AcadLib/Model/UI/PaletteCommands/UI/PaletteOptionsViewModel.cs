using System;
using System.Reactive.Linq;
using AcadLib.Properties;
using NetLib.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AcadLib.UI.PaletteCommands.UI
{
    public class PaletteOptionsViewModel : BaseViewModel
    {
        [Reactive] public double FontSize { get; set; }
        [Reactive] public double ImageSize { get; set; }
        [Reactive] public bool IsImageAndText { get; set; }
        [Reactive] public bool IsList { get; set; }
        [Reactive] public bool IsOnlyImage { get; set; }

        public PaletteOptionsViewModel()
        {
            ImageSize = Settings.Default.PaletteImageSize;
            this.WhenAnyValue(v => v.ImageSize).Skip(1).Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(s => Settings.Default.PaletteImageSize = s);
            FontSize = Settings.Default.PaletteFontSize;
            this.WhenAnyValue(v => v.FontSize).Skip(1).Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(s => Settings.Default.PaletteFontSize = s);
            SwitchRadioContent();
            this.WhenAnyValue(v => v.IsOnlyImage).Skip(1).Throttle(TimeSpan.FromMilliseconds(500))
                .Where(w => w).Subscribe(s => SetListStyle(0));
            this.WhenAnyValue(v => v.IsImageAndText).Skip(1).Throttle(TimeSpan.FromMilliseconds(500))
                .Where(w => w).Subscribe(s => SetListStyle(1));
            this.WhenAnyValue(v => v.IsList).Skip(1).Throttle(TimeSpan.FromMilliseconds(500))
                .Where(w => w).Subscribe(s => SetListStyle(2));
        }

        public override void OnClosing()
        {
            Settings.Default.Save();
            base.OnClosing();
        }

        private static void SetListStyle(int listStyle)
        {
            Settings.Default.PaletteStyle = listStyle;
        }

        private void SwitchRadioContent()
        {
            switch (Settings.Default.PaletteStyle)
            {
                case 0:
                    IsOnlyImage = true;
                    break;

                case 1:
                    IsImageAndText = true;
                    break;

                case 2:
                    IsList = true;
                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}