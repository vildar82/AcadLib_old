using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace AcadLib.Layers.AutoLayers
{
    public abstract class AutoLayer
    {
        protected List<string> Commands { get; set; }

        public LayerInfo Layer { get; set; }

        [NotNull]
        public string GetInfo()
        {
            return $"{Layer.Name} - {string.Join(",", Commands)}";
        }

        public abstract List<ObjectId> GetAutoLayerEnts(List<ObjectId> idAddedEnts);
        public abstract bool IsAutoLayerCommand(string globalCommandName);
    }
}
