using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AcadLib.UI.Ribbon.Data
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using AutoCAD_PIK_Manager.Settings;
    using Elements;
    using JetBrains.Annotations;
    using NetLib;

    /// <summary>
    /// Набор вкладок ленты от одной группы пользователя
    /// </summary>
    public class RibbonGroupData
    {
        /// <summary>
        /// Вкладки
        /// </summary>
        public List<RibbonTabData> Tabs { get; set; }

        public List<RibbonItemData> FreeItems { get; set; }

        [NotNull]
        public static Type[] GetTypes()
        {
            return new[]
            {
                typeof(RibbonCommand), typeof(RibbonInsertBlock),
                typeof(RibbonVisualInsertBlock), typeof(RibbonVisualGroupInsertBlock), typeof(RibbonBreak),
                typeof(RibbonSplit), typeof(RibbonToggle)
            };
        }

        [NotNull]
        public static string GetRibbonFile([NotNull] string userGroup)
        {
            return Path.Combine(PikSettings.LocalSettingsFolder, $@"Ribbon\{userGroup}\{userGroup}.ribbon");
        }

        public static string GetImagesFolder(string userGroup)
        {
            var ribbonFile = GetRibbonFile(userGroup);
            var ribbonDir = Path.GetDirectoryName(ribbonFile);
            return Path.Combine(ribbonDir, "Images");
        }

        [NotNull]
        public static RibbonGroupData Load([NotNull] string ribbonFile)
        {
            return ribbonFile.FromXml<RibbonGroupData>(GetTypes());
        }

        public void Save([NotNull] string ribbonFile)
        {
            this.ToXml(ribbonFile, GetTypes());
        }

        public static void SaveImage(string imageSrcFile, string imageName, string userGroup)
        {
            var imageDir = GetImagesFolder(userGroup);
            var imageDestFile = Path.Combine(imageDir, imageName);
            var img = Image.FromFile(imageSrcFile);
            var resizeImg = NetLib.Images.ImageExt.ResizeImage(img, 64, 64);
            resizeImg.Save(imageDestFile, ImageFormat.Png);
        }

        public static void SaveImage(ImageSource imageSrc, string imageName, string userGroup)
        {
            var imageDir = GetImagesFolder(userGroup);
            var resizeImg = NetLib.Images.ImageExt.ResizedImage(imageSrc, 64, 64, 0);
            var file = Path.Combine(imageDir, imageName);
            var fi = new FileInfo(file);
            using (var fileStream = fi.Create())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(resizeImg));
                encoder.Save(fileStream);
            }
        }

        public static string GetImageName(string name)
        {
            name = NetLib.IO.Path.GetValidFileName(name);
            return $"{name}.png";
        }

        public static ImageSource LoadImage(string userGroup, string imageName)
        {
            var imagesDir = GetImagesFolder(userGroup);
            var imageFile = Path.Combine(imagesDir, imageName);
            return new BitmapImage(new Uri(imageFile));
        }
    }
}
