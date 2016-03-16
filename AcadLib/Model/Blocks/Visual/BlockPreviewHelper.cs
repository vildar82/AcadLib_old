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
            var imgsrc = CMLContentSearchPreviews.GetBlockTRThumbnail(btr);
            //System.Drawing.Image res = ImageSourceToGDI(imgsrc as BitmapSource);
            return imgsrc;
        }

        private static System.Drawing.Image ImageSourceToGDI(BitmapSource src)
        {
            var ms = new MemoryStream();
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));
            encoder.Save(ms);
            ms.Flush();
            return System.Drawing.Image.FromStream(ms);
        }
    }
}

