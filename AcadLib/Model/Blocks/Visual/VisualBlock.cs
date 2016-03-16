using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Blocks.Visual
{
    public class VisualBlock : IVisualBlock
    {
        public string Name { get; set; }        
        public ImageSource Image { get; set; }

        public VisualBlock(BlockTableRecord btr)
        {
            Name = btr.Name;            
            Image = BlockPreviewHelper.GetPreview(btr);
        }
    }
}
