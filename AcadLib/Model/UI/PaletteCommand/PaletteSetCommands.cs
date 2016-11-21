using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

namespace AcadLib.PaletteCommands
{
    public class PaletteSetCommands :PaletteSet
    {
        private static PaletteSetCommands _paletteSet;
        private static readonly Guid PaletteGuid = new Guid("623e4502-7407-4566-9d71-3ecbda06b088");

        /// <summary>
        /// Данные для палитры
        /// </summary>
        private List<PaletteModel> models { get; set; }        

        /// <summary>
        /// Команды переданные из сборки данного раздела
        /// </summary>
        public static List<IPaletteCommand> CommandsAddin { get; set; } 

        public PaletteSetCommands() : base(AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup, PaletteGuid)
        {
            Icon = Properties.Resources.pik;
            loadPalettes();           
            // Установка фона контрола на палитре - в зависимости от цветовой темы автокада.            
            CheckTheme();
            Application.SystemVariableChanged += (s, e) =>
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
        public static void InitPalette(List<IPaletteCommand> commands)
        {
            CommandsAddin = commands;
            Commands.AllCommandsCommon();
            SetTrayIcon();
        }

        /// <summary>
        /// Создание палитры и показ
        /// </summary>
        public static void Start()
        {
            if (_paletteSet == null)
            {
                _paletteSet = Create();
            }
            _paletteSet.Visible = true;
        }

        private void loadPalettes()
        {
            models = new List<PaletteModel>();
            var commands = CommandsAddin;
            // Группировка команд
            string groupCommon = "Общие";
            List<IPaletteCommand> commonCommands = Commands.CommandsPalette;
            var groupCommands = commands.GroupBy(c => c.Group).OrderBy(g=>g.Key);
            foreach (var group in groupCommands)
            {
                if (group.Key.Equals(groupCommon, StringComparison.OrdinalIgnoreCase))
                {
                    commonCommands.AddRange(group.ToList());
                }
                else
                {
                    var model = new PaletteModel(group);
                    if (model.PaletteCommands.Any())
                    {
                        CommandsControl commControl = new CommandsControl();
                        commControl.DataContext = model;
                        string name = group.Key;
                        if (string.IsNullOrEmpty(name)) name = "Главная";
                        AddVisual(name, commControl);
                        models.Add(model);
                    }
                }
            }
            // Общие команды для всех отделов определенные в этой сборке            
            var modelCommon = new PaletteModel(commonCommands);
            var controlCommon = new CommandsControl();
            controlCommon.DataContext = modelCommon;
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
            var isDarkTheme = (short)Application.GetSystemVariable("COLORTHEME") == 0;
            System.Windows.Media.Brush colorBkg;
            if (isDarkTheme)
                colorBkg = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 92, 92, 92));
            else
                colorBkg = System.Windows.Media.Brushes.White;
            models.ForEach(m => m.Background = colorBkg);
        }

        private static void SetTrayIcon()
        {
            // Добавление иконки в трей    
            try
            {
                //TrayItem ti = new TrayItem();                
                //ti.ToolTipText = "Палитра ПИК";
                //ti.Icon = Icon.FromHandle(Properties.Resources.logo.GetHicon());
                //ti.MouseDown += PikTray_MouseDown;                
                //ti.Visible = true;                
                //Application.StatusBar.TrayItems.Add(ti);                

                Pane pane = new Pane();                
                pane.ToolTipText = "Палитра ПИК";                                
                pane.Icon = Icon.FromHandle(Properties.Resources.logo.GetHicon());
                pane.MouseDown += PikTray_MouseDown;
                Application.StatusBar.Panes.Add(pane);
            }
            catch { }
        }

        private static void PikTray_MouseDown(object sender, StatusBarMouseDownEventArgs e)
        {
            Start();
        }        
    }
}
