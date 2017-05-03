using System.Windows.Media;
using Autodesk.AutoCAD.DatabaseServices;
using MicroMvvm;

namespace AcadLib.Blocks.Visual
{
    public class VisualBlock : IVisualBlock
    {
        public VisualBlock(BlockTableRecord btr)
        {
            Name = btr.Name;            
            Image = BlockPreviewHelper.GetPreview(btr);
            Redefine = new RelayCommand(OnRedefineBlockExecute, CanRedefineBlockExecute);            
        }
        public RelayCommand Redefine { get; set; }

        public string Name { get; set; }
        public ImageSource Image { get; set; }
        public string File { get; set; }        

        private bool CanRedefineBlockExecute()
        {
            return Block.HasBlockThisDrawing(Name);
        }

        private void OnRedefineBlockExecute()
        {
            VisualInsertBlock.Redefine(this);
        }
    }
}
