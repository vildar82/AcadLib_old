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
            Icon = Properties.Resources.pik_logo;
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
            var groupCommands = commands.GroupBy(c => c.Group).OrderBy(g=>g.Key);
            foreach (var group in groupCommands)
            {
                var model = new PaletteModel(group);
                CommandsControl commControl = new CommandsControl();
                commControl.DataContext = model;
                string name = group.Key;
                if (string.IsNullOrEmpty(name)) name = "Главная";
                AddVisual(name, commControl);
                models.Add(model);
            }
            // Общие команды для всех отделов определенные в этой сборке
            commands = Commands.CommandsPalette;
            var modelCommon = new PaletteModel(commands);
            var controlCommon = new CommandsControl();
            controlCommon.DataContext = modelCommon;
            AddVisual("Общие", controlCommon);
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
            Pane pane = new Pane();
            pane.ToolTipText = "Палитра ПИК";
            pane.Icon = Properties.Resources.pik_logo;
            pane.MouseDown += PikTray_MouseDown;
            Application.StatusBar.Panes.Add(pane);
        }

        private static void PikTray_MouseDown(object sender, StatusBarMouseDownEventArgs e)
        {
            Start();
        }        
    }
}
