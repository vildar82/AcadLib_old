using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.Blocks.Visual
{
    public class VisualModel
    {
        public ObservableCollection<VisualBlock> Visuals { get; private set; }

        public VisualModel()
        {
            Visuals = new ObservableCollection<VisualBlock>();
        }

        public void LoadData(List<VisualBlock> visualBlocks)
        {
            foreach (var item in visualBlocks)
            {
                Visuals.Add(item);
            }
        }
    }
}
