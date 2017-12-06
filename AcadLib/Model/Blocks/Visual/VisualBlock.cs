using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media;
using MicroMvvm;

namespace AcadLib.Blocks.Visual
{
    public class VisualBlock : IVisualBlock
    {
        public VisualBlock([NotNull] BlockTableRecord btr)
        {
            Name = btr.Name;
            Image = BlockPreviewHelper.GetPreview(btr);
            Redefine = new RelayCommand(RedefineBlockExecute, CanRedefineBlockExecute);
        }
        public ICommand Redefine { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        public string File { get; set; }

        private bool CanRedefineBlockExecute()
        {
            return Block.HasBlockThisDrawing(Name);
        }

        private void RedefineBlockExecute()
        {
            VisualInsertBlock.Redefine(this);
        }
    }
}
