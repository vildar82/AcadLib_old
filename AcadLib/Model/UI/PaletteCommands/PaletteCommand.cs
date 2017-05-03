using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using MicroMvvm;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib.PaletteCommands
{
    public class PaletteCommand : IPaletteCommand
    {
        public string Description { get; set; }
        public ImageSource Image { get; set; }
        /// <summary>
        /// Короткое название кнопки
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Имя команды AutoCAD
        /// </summary>
        public string CommandName { get; set; }
        /// <summary>
        /// Группа команд - для объекдинения в палитры
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// Ограниечение доступа по логину
        /// </summary>
        public List<string> Access { get; set; }
        /// <summary>
        /// Индекс кнопки на палитре
        /// </summary>
        public int Index { get; set; }
        public string HelpMedia { get; set; }
        public List<MenuItemCommand> ContexMenuItems { get; set; }
        public ICommand Command { get; set; }

        //public PaletteCommand() { }

        public PaletteCommand(string name, Bitmap image, string command, string description, string group = "")
        {   
            Image = GetSource(image);
            Name = name;
            CommandName = command;
            Command = new RelayCommand(Execute);
            Description = description;
            Group = group;
            // HelpMedia
            HelpMedia = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder,
                AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup, "Help", command, command + ".mp4");
            if (!File.Exists(HelpMedia))
            {
                HelpMedia = null;
            }
        }

        public PaletteCommand(List<string> access, string name, Bitmap image, string command, string description, string group = "")            
            :this(name, image, command, description, group)
        {
            Access = access;
        }

        public virtual void Execute()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            using (doc.LockDocument())
            {
                doc.SendStringToExecute(CommandName + " ", true, false, true);
            }
        }

        public ImageSource GetSource(Bitmap image)
        {
            if (image == null)
            {
                image = Properties.Resources.unknown;
            }
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    image.GetHbitmap(),
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
