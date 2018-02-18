using JetBrains.Annotations;
using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

// ReSharper disable once CheckNamespace
namespace AcadLib.PaletteCommands
{
    public class PaletteCommand : ReactiveObject, IPaletteCommand
    {
        /// <summary>
        /// Ограниечение доступа по логину
        /// </summary>
        public List<string> Access { get; set; }
        public ICommand Command { get; set; }
        /// <summary>
        /// Имя команды AutoCAD
        /// </summary>
        public string CommandName { get; set; }
        public List<MenuItemCommand> ContexMenuItems { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Группа команд - для объекдинения в палитры
        /// </summary>
        public string Group { get; set; }
        public string HelpMedia { get; set; }
        public ImageSource Image { get; set; }
        /// <summary>
        /// Индекс кнопки на палитре
        /// </summary>
        public int Index { get; set; }
        public bool IsTest { get; set; }
        /// <summary>
        /// Короткое название кнопки
        /// </summary>
        public string Name { get; set; }
        //public PaletteCommand() { }

        public PaletteCommand()
        {

        }

        public PaletteCommand(string name, Bitmap image, string command, string description, string group = "",
            bool isTest = false)
        {
            IsTest = isTest;
            Image = GetSource(image, isTest);
            Name = name;
            CommandName = command;
            Command = new RelayCommand(Execute);
            Description = $"{description} {Environment.NewLine}{command}";
            Group = group;
            if (isTest)
            {
                Name += " (Тест)";
            }
            // HelpMedia
            HelpMedia = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder,
                AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup, "Help", command, command + ".mp4");
            if (!File.Exists(HelpMedia))
            {
                HelpMedia = null;
            }
        }

        public PaletteCommand(List<string> access, string name, Bitmap image,
            string command, string description, string group = "", bool isTest = false)
            : this(name, image, command, description, group, isTest)
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

        private static void AddTestWaterMark([NotNull] Image image)
        {
            using (var watermarkImage = Properties.Resources.test)
            using (var imageGraphics = Graphics.FromImage(image))
            using (var watermarkBrush = new TextureBrush(watermarkImage))
            {
                var x = image.Width / 2 - watermarkImage.Width / 2;
                var y = image.Height / 2 - watermarkImage.Height / 2;
                watermarkBrush.TranslateTransform(x, y);
                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
            }
        }

        [NotNull]
        private static ImageSource GetSource(Bitmap image, bool isTest)
        {
            if (image == null)
            {
                image = Properties.Resources.unknown;
            }
            if (isTest)
            {
                AddTestWaterMark(image);
            }
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    image.GetHbitmap(),
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }
    }
}