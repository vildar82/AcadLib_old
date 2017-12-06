using JetBrains.Annotations;
using NetLib.WPF;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AcadLib.Blocks.Visual.UI
{
    public class VisualBlocksViewModel : BaseViewModel
    {
        public VisualBlocksViewModel()
        {

        }

        public VisualBlocksViewModel([NotNull] List<IVisualBlock> visuals)
        {
            Groups = visuals.GroupBy(g => g.Group).Select(s => new VisualGroup { Name = s.Key, Blocks = s.ToList() }).ToList();
            Insert = ReactiveCommand.Create<IVisualBlock>(OnInsertExecute);
            VisibleSeparator = Groups.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        public List<VisualGroup> Groups { get; set; }
        public ReactiveCommand Insert { get; set; }
        public Visibility VisibleSeparator { get; set; }

        private void OnInsertExecute(IVisualBlock block)
        {
            DialogResult = true;
            VisualInsertBlock.Insert(block);
        }
    }
}
