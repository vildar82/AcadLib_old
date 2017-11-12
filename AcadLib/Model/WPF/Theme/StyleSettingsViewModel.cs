using MahApps.Metro;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace AcadLib.WPF.Theme
{
    public class StyleSettingsViewModel : BaseViewModel
    {
        public StyleSettingsViewModel(BaseViewModel parent) : base(parent)
        {
            Themes = ThemeManager.AppThemes.Select(s => new ThemeViewModel(s)).ToList();
            Accents = ThemeManager.Accents.Select(s => new AccentViewModel(s)).ToList();
            SelectedTheme = Themes.FirstOrDefault(t => t.Theme == StyleSettings.Theme);
            SelectedAccent = Accents.FirstOrDefault(t => t.Accent == StyleSettings.Accent);
            this.WhenAnyValue(w => w.SelectedTheme, w => w.SelectedAccent).Skip(1).Subscribe(s =>
            {
                StyleSettings.Theme = SelectedTheme.Theme;
                StyleSettings.Accent = SelectedAccent.Accent;
                ThemeManager.ChangeAppStyle(Window.Resources, StyleSettings.Accent, StyleSettings.Theme);
            });
        }

        public List<ThemeViewModel> Themes { get; set; }
        [Reactive] public ThemeViewModel SelectedTheme { get; set; }
        public List<AccentViewModel> Accents { get; set; }
        [Reactive] public AccentViewModel SelectedAccent { get; set; }

        public override void OnClosed()
        {
            StyleSettings.Save();
        }
    }
}
