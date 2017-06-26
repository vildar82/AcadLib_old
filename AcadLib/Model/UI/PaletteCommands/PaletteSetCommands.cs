using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;
using Brush = System.Windows.Media.Brush;

namespace AcadLib.PaletteCommands
{
    public class PaletteSetCommands :PaletteSet
    {
        private static PaletteSetCommands _paletteSet;
        private static readonly Guid PaletteGuid = new Guid("623e4502-7407-4566-9d71-3ecbda06b088");
        private static string commandStartPalette;
	    private static string versionPalette;

        /// <summary>
        /// Данные для палитры
        /// </summary>
        private List<PaletteModel> models { get; set; }        

        /// <summary>
        /// Команды переданные из сборки данного раздела
        /// </summary>
        public static List<IPaletteCommand> CommandsAddin { get; set; } 

        public PaletteSetCommands() : 
            base(AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup, commandStartPalette, PaletteGuid)
        {
            Icon = Properties.Resources.pik;
            loadPalettes();           
            // Установка фона контрола на палитре - в зависимости от цветовой темы автокада.            
            CheckTheme();
            Autodesk.AutoCAD.ApplicationServices.Core.Application.SystemVariableChanged += (s, e) =>
            {
                if (e.Name == "COLORTHEME" && e.Changed)
                    CheckTheme();
            };
        }

        /// <summary>
        /// Подготовка для определения палитры ПИК.
        /// Добавление значка ПИК в трей для запуска палитры.
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="commandStartPalette">Имя команды для старта палитры</param>
        public static void InitPalette(List<IPaletteCommand> commands, string commandStartPalette)
        {
            try
            {
                PaletteSetCommands.commandStartPalette = commandStartPalette;
                CommandsAddin = commands;
                Commands.AllCommandsCommon();
                SetTrayIcon();
                var userGroups = string.Join(",", AutoCAD_PIK_Manager.Settings.PikSettings.UserGroupsCombined);
	            versionPalette = $"{Assembly.GetCallingAssembly().GetName().Version} {userGroups}";

            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex, $"AcadLib.PaletteCommands.InitPalette() - {commandStartPalette}.");
            }
        }

        /// <summary>
        /// Создание палитры и показ
        /// </summary>
        public static void Start()
        {
            try
            {
                if (_paletteSet == null)
                {
                    _paletteSet = Create();
                }
                _paletteSet.Visible = true;
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex, "PaletteSetCommands.Start().");
            }
        }

        private void loadPalettes()
        {
            models = new List<PaletteModel>();
            // Группировка команд
            var groupCommon = "Общие";
            var commonCommands = Commands.CommandsPalette;
            var groupCommands = CommandsAddin.GroupBy(c => c.Group).OrderBy(g=>g.Key);
            foreach (var group in groupCommands)
            {
                if (group.Key.Equals(groupCommon, StringComparison.OrdinalIgnoreCase))
                {
                    commonCommands.AddRange(group.ToList());
                }
                else
                {
                    var model = new PaletteModel(group, versionPalette);
                    if (model.PaletteCommands.Any())
                    {
	                    var commControl = new UI.CommandsControl {DataContext = model};
	                    var name = group.Key;
                        if (string.IsNullOrEmpty(name)) name = "Главная";
                        AddVisual(name, commControl);
                        models.Add(model);
                    }
                }
            }
            // Общие команды для всех отделов определенные в этой сборке            
            var modelCommon = new PaletteModel(commonCommands, versionPalette);
	        var controlCommon = new UI.CommandsControl {DataContext = modelCommon};
	        AddVisual(groupCommon, controlCommon);
            models.Add(modelCommon);
        }

	    private static PaletteSetCommands Create()
        {
            var palette = new PaletteSetCommands();
            return palette;
        }

        private void CheckTheme()
        {
            var isDarkTheme = (short)Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("COLORTHEME") == 0;
	        Brush colorBkg = isDarkTheme ? new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 92, 92, 92)) :
		        System.Windows.Media.Brushes.White;
            models.ForEach(m => m.Background = colorBkg);
        }

        private static void SetTrayIcon()
        {
            // Добавление иконки в трей    
            try
            {
	            var ti = new TrayItem
	            {
		            ToolTipText = "Палитра ПИК",
		            Icon = Icon.FromHandle(Properties.Resources.logo.GetHicon())
	            };
	            ti.MouseDown += PikTray_MouseDown;
                ti.Visible = true;
                Application.StatusBar.TrayItems.Add(ti);

                //Pane pane = new Pane();                
                //pane.ToolTipText = "Палитра ПИК";                                
                //pane.Icon = Icon.FromHandle(Properties.Resources.logo.GetHicon());
                //pane.MouseDown += PikTray_MouseDown;
                //Application.StatusBar.Panes.Add(pane);
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex, "PaletteSetCommands.SetTrayIcon().");
            }
        }

        private static void PikTray_MouseDown(object sender, StatusBarMouseDownEventArgs e)
        {
            Start();
        }        
    }
}
