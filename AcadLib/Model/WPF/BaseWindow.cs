using AcadLib.WPF.Theme;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AcadLib.WPF
{
    public class BaseWindow : MetroWindow
    {
        protected readonly BaseViewModel model;

        public BaseWindow() : this(null)
        {
            
        }

        public BaseWindow(BaseViewModel model)
        {
            this.model = model;
            var hideBinding = new Binding("Hide");
            SetBinding(VisibilityHelper.IsHiddenProperty, hideBinding);
            var dialogRegBinding = new Binding { Source = model };
            SetBinding(DialogParticipation.RegisterProperty, dialogRegBinding);
            WindowStartupLocation = model?.Parent?.Window == null
                ? WindowStartupLocation.CenterScreen
                : WindowStartupLocation.CenterOwner;
        }

        protected override void OnInitialized(EventArgs e)
        {
            DataContext = model;
            AddStyleResouse();
            // Применение темы оформления
            ApplyTheme();
            StyleSettings.Change += (s, a) => ApplyTheme();
            if (model != null) model.Window = this;
            SaveWindowPosition = true;
            GlowBrush = Resources["AccentColorBrush"] as Brush;
            if (!(model is StyleSettingsViewModel))
            {
                var buttonTheme = new Button
                {
                    Content = new PackIconOcticons { Kind = PackIconOcticonsKind.Paintcan },
                    ToolTip = "Настройка тем оформления окон"
                };
                buttonTheme.Click += ButtonTheme_Click;
                if (RightWindowCommands == null)
                {
                    RightWindowCommands = new WindowCommands();
                }
                RightWindowCommands.Items.Add(buttonTheme);
            }
            base.OnInitialized(e);
        }

        private void ApplyTheme()
        {
            ThemeManager.ChangeAppStyle(Resources, StyleSettings.Accent, StyleSettings.Theme);
        }

        private static void ButtonTheme_Click(object sender, RoutedEventArgs e)
        {
            var styleSettingsVM = new StyleSettingsViewModel();
            var styleSettingsView = new StyleSettingsView(styleSettingsVM);
            if (styleSettingsView.ShowDialog() == true)
            {
                StyleSettings.Save();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            model?.OnClosed();
            base.OnClosed(e);
        }

        private void AddStyleResouse()
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/AcadLib;component/Model/WPF/Style.xaml")
            });
        }
    }
}
