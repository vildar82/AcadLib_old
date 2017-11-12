using System;
using MahApps.Metro;

namespace AcadLib.WPF
{
    public static class StyleSettings
    {
        static StyleSettings()
        {
            Accent = ThemeManager.GetAccent(Properties.Settings.Default.Accent);
            Theme = ThemeManager.GetAppTheme(Properties.Settings.Default.Theme);
        }

        public static Accent Accent { get; set; }
        public static AppTheme Theme { get; set; }
        public static event EventHandler Change;

        public static void Save()
        {
            Properties.Settings.Default.Accent = Accent?.Name ?? "Blue";
            Properties.Settings.Default.Theme = Theme?.Name ?? "BaseLight";
            Properties.Settings.Default.Save();
            Change?.Invoke(null, EventArgs.Empty);
        }
    }
}
