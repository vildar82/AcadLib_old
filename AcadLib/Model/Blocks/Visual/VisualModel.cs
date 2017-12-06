using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AcadLib.Blocks.Visual
{
    public class VisualModel
    {
        public ObservableCollection<VisualBlock> Visuals { get; private set; }

        public VisualModel()
        {
            Visuals = new ObservableCollection<VisualBlock>();
        }

        public void LoadData([NotNull] List<VisualBlock> visualBlocks)
        {
            foreach (var item in visualBlocks)
            {
                Visuals.Add(item);
            }
        }
    }
}
