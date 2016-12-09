using MicroMvvm;
using NetLib.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.Blocks.Visual.UI
{
    public class VisualBlocksViewModel : ViewModelBase
    {        
        public VisualBlocksViewModel()
        {

        }
        public VisualBlocksViewModel(List<IVisualBlock> visuals)
        {
            Visuals = new ObservableCollection<IVisualBlock>(visuals);            
            Insert = new RelayCommand<IVisualBlock>(OnInsertExecute);
        }        

        public ObservableCollection<IVisualBlock> Visuals { get; set; }        
        public RelayCommand<IVisualBlock> Insert { get; set; }

        private void OnInsertExecute(IVisualBlock block)
        {
            VisualInsertBlock.Insert(block);
        }        
    }
}
