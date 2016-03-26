using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows.Data;

namespace AcadLib.Blocks.Visual
{
    public static class BlockPreviewHelper
    {
        public static ImageSource GetPreview(BlockTableRecord btr)
        {
            return CMLContentSearchPreviews.GetBlockTRThumbnail(btr);            
        }

        public static System.Drawing.Image GetPreviewImage(BlockTableRecord btr)
        {
            var imgsrc = CMLContentSearchPreviews.GetBlockTRThumbnail(btr);            
            return ImageSourceToGDI((BitmapSource)imgsrc);            
        }

        public static Icon GetPreviewIcon(BlockTableRecord btr)
        {
            var imgsrc = CMLContentSearchPreviews.GetBlockTRThumbnail(btr);
            var bitmap = ImageSourceToGDI((BitmapSource)imgsrc) as Bitmap;
            var iconPtr = bitmap.GetHicon();
            return Icon.FromHandle(iconPtr);
        }

        private static System.Drawing.Image ImageSourceToGDI(BitmapSource src)
        {
            using (var ms = new MemoryStream())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(src));
                encoder.Save(ms);
                ms.Flush();
                return System.Drawing.Image.FromStream(ms);
            }
        }        
    }
}

