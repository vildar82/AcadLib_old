using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace AcadLib.Layers.AutoLayers
{
    /// <summary>
    /// Авто-слои для размеров
    /// </summary>
    public class AutoLayerDim : AutoLayer
    {
        public AutoLayerDim()
        {
            Layer = new LayerInfo($"{LayerExt.GroupLayerPrefix}_Размеры");
            Commands = new List<string> { "DIM" };
        }

        public override bool IsAutoLayerCommand(string globalCommandName)
        {
            return globalCommandName.StartsWith("DIM");
        }

        public override List<ObjectId> GetAutoLayerEnts(List<ObjectId> idAddedEnts)
        {
            return idAddedEnts.Where(w => IsDimEnt(w)).ToList();
        }       

        private bool IsDimEnt(ObjectId idEnt)
        {
            return idEnt.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Dimension)));
        }
    }
}
