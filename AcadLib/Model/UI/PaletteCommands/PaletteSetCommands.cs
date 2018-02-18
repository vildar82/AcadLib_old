using Autodesk.AutoCAD.Windows;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using AcadLib.Properties;
using AcadLib.UI.PaletteCommands.UI;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Brush = System.Windows.Media.Brush;

// ReSharper disable once CheckNamespace
namespace AcadLib.PaletteCommands
{
    [PublicAPI]
    public class PaletteSetCommands : PaletteSet
    {
        internal static readonly List<UserGroupPalette> _paletteSets = new List<UserGroupPalette>();
        private readonly string versionPalette;

        /// <summary>
        /// Команды переданные из сборки данного раздела
        /// </summary>
        public List<IPaletteCommand> CommandsAddin { get; set; }
        /// <summary>
        /// Данные для палитры
        /// </summary>
        private List<PaletteModel> Models { get; set; }

        public PaletteSetCommands(string paletteName, Guid paletteGuid, string commandStartPalette,
            List<IPaletteCommand> commandsAddin, string versionPalette) :
            base(paletteName, commandStartPalette, paletteGuid)
        {
            this.versionPalette = versionPalette;
            CommandsAddin = commandsAddin;
            Icon = Resources.pik;
            LoadPalettes();
            PaletteAddContextMenu += PaletteSetCommands_PaletteAddContextMenu;
        }

        public static double GetButtonWidth()
        {
            if (Settings.Default.PaletteStyle == 1)
            {
                var wb = MathExt.Interpolate(8, 55, 25, 180, Settings.Default.PaletteFontSize);
                return wb < Settings.Default.PaletteImageSize ? Settings.Default.PaletteImageSize : wb;
            }
            return Settings.Default.PaletteImageSize * 1.08;
        }

        public static double GetFontSize()
        {
            return Settings.Default.PaletteFontSize * 2.5;
        }

        /// <summary>
        /// Подготовка для определения палитры ПИК.
        /// Добавление значка ПИК в трей для запуска палитры.
        /// </summary>
        public static void InitPalette(List<IPaletteCommand> commands, string commandStartPalette,
            string paletteName, Guid paletteGuid)
        {
            try
            {
                var palette = _paletteSets.FirstOrDefault(p => p.Guid.Equals(paletteGuid));
                if (palette == null)
                {
                    commands.AddRange(Commands.CommandsPalette);
                    var ver = Assembly.GetCallingAssembly().GetName().Version;
                    _paletteSets.Add(new UserGroupPalette
                    {
                        Guid = paletteGuid,
                        Name = paletteName,
                        CommandStartPalette = commandStartPalette,
                        Commands = commands,
                        VersionPalette = ver.ToString()
                    });
                    SetTrayIcon(paletteName, paletteGuid, ver);
                }
                else
                {
                    palette.Commands.AddRange(commands);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"AcadLib.PaletteCommands.InitPalette() - {commandStartPalette}.");
            }
        }

        public static bool IsAccess([CanBeNull] List<string> accessLogins)
        {
            return accessLogins == null ||
                   accessLogins.Contains(Environment.UserName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Создание палитры и показ
        /// </summary>
        public static void Start(Guid paletteGuid)
        {
            try
            {
                var paletteUserGroup = _paletteSets.FirstOrDefault(p => p.Guid.Equals(paletteGuid));
                if (paletteUserGroup == null) return;
                if (paletteUserGroup.Palette == null)
                {
                    paletteUserGroup.Palette = new PaletteSetCommands(paletteUserGroup.Name, paletteUserGroup.Guid,
                        paletteUserGroup.CommandStartPalette, paletteUserGroup.Commands, paletteUserGroup.VersionPalette);
                }
                paletteUserGroup.Palette.Visible = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "PaletteSetCommands.Start().");
            }
        }

        private static void PikTray_MouseDown(Guid paletteGuid)
        {
            Start(paletteGuid);
        }

        private static void SetTrayIcon(string paletteName, Guid paletteGuid, Version ver)
        {
            // Добавление иконки в трей
            try
            {
                var p = new Pane
                {
                    ToolTipText = $"Палитра {paletteName}, вер. {ver.Revision}",
                    Icon = Icon.FromHandle(Resources.logo.GetHicon())
                };
                p.MouseDown += (o, e) => PikTray_MouseDown(paletteGuid);
                p.Visible = false;
                Application.StatusBar.Panes.Insert(0, p);
                p.Visible = true;
                Application.StatusBar.Update();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "PaletteSetCommands.SetTrayIcon().");
            }
        }

        private void CheckTheme()
        {
            var isDarkTheme = (short)Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("COLORTHEME") == 0;
            Brush colorBkg = isDarkTheme ? new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 92, 92, 92)) :
                System.Windows.Media.Brushes.White;
            Models.ForEach(m => m.Background = colorBkg);
        }

        private void LoadPalettes()
        {
            Models = new List<PaletteModel>();
            // Группировка команд
            //const string groupCommon = Commands.GroupCommon;
            //var commonCommands = Commands.CommandsPalette;
            var groupCommands = CommandsAddin.GroupBy(c => c.Group).OrderBy(g => g.Key);
            foreach (var group in groupCommands)
            {
                //if (group.Key.Equals(groupCommon, StringComparison.OrdinalIgnoreCase))
                //{
                //    commonCommands.AddRange(group);
                //}
                //else
                {
                    var model = new PaletteModel(group.GroupBy(g => g.Name).Select(s => s.First()), versionPalette);
                    if (model.PaletteCommands.Any())
                    {
                        var commControl = new UI.CommandsControl { DataContext = model };
                        var name = group.Key;
                        if (string.IsNullOrEmpty(name)) name = "Главная";
                        AddVisual(name, commControl);
                        Models.Add(model);
                    }
                }
            }
            //// Общие команды для всех отделов определенные в этой сборке
            //var modelCommon = new PaletteModel(commonCommands.GroupBy(g => g.Name).Select(s => s.First()).ToList(),
            //    versionPalette);
            //var controlCommon = new UI.CommandsControl { DataContext = modelCommon };
            //AddVisual(groupCommon, controlCommon);
            //Models.Add(modelCommon);
            Settings.Default.PropertyChanged += (o, e) =>
            {
                double bw;
                switch (e.PropertyName)
                {
                    case "PaletteImageSize":
                    case "PaletteStyle":
                        bw = GetButtonWidth();
                        Models.ForEach(m => m.ButtonWidth = bw);
                        break;

                    case "PaletteFontSize":
                        var fontMaxH = GetFontSize();
                        bw = GetButtonWidth();
                        Models.ForEach(m =>
                        {
                            m.FontMaxHeight = fontMaxH;
                            m.ButtonWidth = bw;
                        });
                        break;
                }
            };
        }

        private void PaletteSetCommands_PaletteAddContextMenu(object sender, [NotNull] PaletteAddContextMenuEventArgs e)
        {
            var mi = new MenuItem("Параметры отображения");
            mi.Click += (co, ce) =>
            {
                var paletteoptView = new PaletteOptionsView(new PaletteOptionsViewModel(Models));
                paletteoptView.ShowDialog();
            };
            e.MenuItems.Add(mi);
        }
    }

    internal class UserGroupPalette
    {
        public List<IPaletteCommand> Commands { get; set; }
        public string CommandStartPalette { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public PaletteSetCommands Palette { get; set; }
        public string VersionPalette { get; set; }
    }
}