using AcadLib.WPF;
using MicroMvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using NetLib.WPF;

namespace AcadLib.Blocks.Visual.UI
{
    public class VisualBlocksViewModel : BaseViewModel
    {
        public VisualBlocksViewModel()
        {

        }

        public VisualBlocksViewModel(List<IVisualBlock> visuals)
        {
            Groups = visuals.GroupBy(g => g.Group).Select(s => new VisualGroup {Name = s.Key, Blocks = s.ToList()}).ToList();
            Insert = new RelayCommand<IVisualBlock>(OnInsertExecute);
            VisibleSeparator = Groups.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        public List<VisualGroup> Groups { get; set; }
        public RelayCommand<IVisualBlock> Insert { get; set; }
        public Visibility VisibleSeparator { get; set; }

        private void OnInsertExecute(IVisualBlock block)
        {
            DialogResult = true;
            VisualInsertBlock.Insert(block);
        }
    }
}
