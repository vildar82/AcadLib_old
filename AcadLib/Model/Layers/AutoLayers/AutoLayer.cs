using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Layers.AutoLayers
{
    public abstract class AutoLayer
    {        
        public List<string> Commands { get; set; }
        public LayerInfo Layer { get; set; }        

        public string GetInfo()
        {
            return $"{Layer.Name} - {string.Join(",", Commands)}";
        }

        public abstract List<ObjectId> GetAutoLayerEnts(List<ObjectId> idAddedEnts);
        public abstract bool IsAutoLayerCommand(string globalCommandName);        
    }
}
