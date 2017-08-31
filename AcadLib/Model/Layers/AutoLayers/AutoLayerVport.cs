using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using NetLib;

namespace AcadLib.Layers.AutoLayers
{
    /// <summary>
    /// Авто-слои для размеров
    /// </summary>
    public class AutoLayerVport : AutoLayer
    {
        public AutoLayerVport()
        {
            Layer = new LayerInfo($"{LayerExt.GroupLayerPrefix}_Видовой экран");
            Commands = new List<string> { "VPORTS" };
        }

        public override bool IsAutoLayerCommand(string globalCommandName)
        {
            return globalCommandName.EqualsIgnoreCase("VPORTS");
        }

        public override List<ObjectId> GetAutoLayerEnts(List<ObjectId> idAddedEnts)
        {
            return idAddedEnts?.Where(IsVportEnt).ToList();
        }       

        private static bool IsVportEnt(ObjectId idEnt)
        {
	        return idEnt.ObjectClass == General.ClassVport;
        }
    }
}
