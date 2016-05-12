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
        private static Assembly _assCommands;

        public List<PaletteModel> Models { get; set; }

        public PaletteSetCommands() : base(AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup, PaletteGuid)
        {
            Icon = Properties.Resources.pik_logo;

            loadPalettes();           
            
            CheckTheme();
            Application.SystemVariableChanged += (s, e) =>
            {
                if (e.Name == "COLORTHEME" && e.Changed)
                    CheckTheme();
            };
        }

        private void loadPalettes()
        {
            Models = new List<PaletteModel>();
            var commands = GetAllCommands(_assCommands, AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup);
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
                Models.Add(model);
            }
            // Общие команды для всех отделов определенные в этой сборке
            commands = GetAllCommands(Assembly.GetExecutingAssembly(), string.Empty);
            var modelCommon = new PaletteModel(commands);
            var controlCommon = new CommandsControl();
            controlCommon.DataContext = modelCommon;
            AddVisual("Общие", controlCommon);
            Models.Add(modelCommon);
        }

        public static void Start()
        {
            if (_paletteSet == null)
            {   
                _paletteSet = Create();
            }
            _paletteSet.Visible = true;
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
            Models.ForEach(m => m.Background = colorBkg);
        }

        public static void SetTrayIcon(Assembly assm)
        {
            // Добавление иконки в трей                        
            _assCommands = assm;
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

        public List<IPaletteCommand> GetAllCommands(Assembly asm, string groupPik)
        {
            List<IPaletteCommand> commands = new List<IPaletteCommand>();
            if (asm == null) return commands;
            if (!asm.IsDefined(typeof(CommandClassAttribute), false))
            {
                AutoCAD_PIK_Manager.Log.Error($"Загрузка палитры ПИК. Не определен атрибут CommandClass в сборке {asm.FullName}");
                return commands;
            }
            var atrCommandsClass = (CommandClassAttribute)asm.GetCustomAttribute(typeof(CommandClassAttribute));
            var methods = atrCommandsClass.Type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                if (method.IsDefined(typeof(CommandMethodAttribute)))
                {
                    var atrCommand = (CommandMethodAttribute)method.GetCustomAttribute(typeof(CommandMethodAttribute));
                    if (method.IsDefined(typeof(PaletteCommandAttribute)))
                    {
                        var atrPal = (PaletteCommandAttribute)method.GetCustomAttribute(typeof(PaletteCommandAttribute));
                        // определение картинки для кнопки из ресурсов сборки по имени команды
                        Bitmap img;
                        string resourceName = asm.GetName().Name + ".Properties.Resources";
                        var rm = new System.Resources.ResourceManager(resourceName, asm);
                        img = (Bitmap)rm.GetObject(atrCommand.GlobalName);
                        if (img == null)
                            img = Properties.Resources.unknown;

                        PaletteCommand palCom = new PaletteCommand(atrPal.Name, img, atrCommand.GlobalName, atrPal.Description, atrPal.Group);
                        // HelpMedia
                        palCom.HelpMedia = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder,
                            groupPik, "Help", atrCommand.GlobalName, atrCommand.GlobalName + ".mp4");
                        if (!File.Exists(palCom.HelpMedia))
                        {
                            palCom.HelpMedia = null;
                        }
                        commands.Add(palCom);                        
                    }
                }
            }
            return commands;
        }
    }
}
