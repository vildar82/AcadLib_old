using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace AcadLib.Layers
{
    /// <summary>
    /// Состояние слоев - для проверки видимости объектов на чертеже
    /// </summary>
    public class LayerVisibleState
    {
        Dictionary<string, bool> layerVisibleDict;

        /// <summary>
        /// Нужно создавать новый объект LayerVisibleState после возмоного изменения состояния слоев пользователем.
        /// </summary>
        /// <param name="db"></param>
        public LayerVisibleState(Database db)
        {
            layerVisibleDict = GetLayerVisibleState(db);
        }

        [NotNull]
        private Dictionary<string, bool> GetLayerVisibleState([NotNull] Database db)
        {
            var res = new Dictionary<string, bool>();
            var lt = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
            foreach (var idLayer in lt)
            {
                var layer = idLayer.GetObject(OpenMode.ForRead) as LayerTableRecord;
                res.Add(layer.Name, !layer.IsOff && !layer.IsFrozen);
            }
            return res;
        }

        /// <summary>
        /// Объект на видим - не скрыт, не на выключенном или замороженном слое
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public bool IsVisible([NotNull] Entity ent)
        {
            var res = true;
            if (!ent.Visible)
            {
                res = false;
            }
            else
            {
                // Слой выключен или заморожен                
                layerVisibleDict.TryGetValue(ent.Layer, out res);
            }
            return res;
        }
    }
}
