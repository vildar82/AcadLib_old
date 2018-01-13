using System;
using System.Drawing;
using JetBrains.Annotations;

namespace AcadLib.Images
{
    [PublicAPI]
    public static class ImageExt
    {
        [Obsolete("Пустой метод", true)]
        public static void AddWatermark(this Bitmap bitmap, string watermarkText)
        {
            //var font = new Font("Arial", 20, FontStyle.Bold, GraphicsUnit.Pixel);
            //var color = Color.FromArgb(10, 0, 0, 0); //Adds a black watermark with a low alpha value (almost transparent).
            //var atPoint = new Point(100, 100); //The pixel point to draw the watermark at (this example puts it at 100, 100 (x, y)).
            //var brush = new SolidBrush(color);

            //Graphics graphics = null;
            //try
            //{
            //    graphics = Graphics.FromImage(bitmap);
            //}
            //catch
            //{
            //    var temp = bitmap;
            //    bitmap = new Bitmap(bitmap.Width, bitmap.Height);
            //    graphics = Graphics.FromImage(bitmap);
            //    graphics.DrawImage(temp, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
            //    temp.Dispose();
            //}

            //graphics.DrawString(text, font, brush, atPoint);
            //graphics.Dispose();

            //bitmap.Save(outputStream);
        }
    }
}