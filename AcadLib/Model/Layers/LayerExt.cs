using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Layers
{
    public static class LayerExt
    {
        /// <summary>
        /// Получение слоя.
        /// Если его нет в базе, то создается.      
        /// </summary>
        /// <param name="layerInfo">параметры слоя</param>
        /// <returns></returns>
        public static ObjectId GetLayerOrCreateNew(this LayerInfo layerInfo)
        {
            ObjectId idLayer = ObjectId.Null;
            Database db = HostApplicationServices.WorkingDatabase;
            // Если уже был создан слой, то возвращаем его. Опасно, т.к. перед повторным запуском команды покраски, могут удалить/переименовать слой марок.                           
            using (var lt = db.LayerTableId.Open(OpenMode.ForRead) as LayerTable)
            {
                if (lt.Has(layerInfo.Name))
                {
                    idLayer = lt[layerInfo.Name];                    
                }
                else
                {
                    idLayer = CreateLayer(layerInfo, lt);
                }
            }
            return idLayer;
        }

        /// <summary>
        /// Создание слоя.
        /// Слоя не должно быть в таблице слоев.
        /// </summary>
        /// <param name="layerInfo">параметры слоя</param>
        /// <param name="lt">таблица слоев открытая для чтения. Выполняется UpgradeOpen и DowngradeOpen</param>
        public static ObjectId CreateLayer(this LayerInfo layerInfo, LayerTable lt)
        {
            ObjectId idLayer = ObjectId.Null;
            // Если слоя нет, то он создается.            
            using (var newLayer = new LayerTableRecord())
            {
                newLayer.Name = layerInfo.Name;
                newLayer.Color = layerInfo.Color;
                newLayer.IsFrozen = layerInfo.IsFrozen;
                newLayer.IsLocked = layerInfo.IsLocked;
                newLayer.IsOff = layerInfo.IsOff;
                newLayer.IsPlottable = layerInfo.IsPlotable;
                newLayer.LineWeight = layerInfo.LineWeight;
                if (!layerInfo.LinetypeObjectId.IsNull)
                    newLayer.LinetypeObjectId = layerInfo.LinetypeObjectId;
                else if (!string.IsNullOrEmpty(layerInfo.LineType))
                {
                    newLayer.LinetypeObjectId = lt.Database.GetLineTypeIdByName(layerInfo.LineType);
                }
                else
                {
                    newLayer.LinetypeObjectId = lt.Database.GetLineTypeIdContinuous();
                }
                lt.UpgradeOpen();
                idLayer = lt.Add(newLayer);
                lt.DowngradeOpen();
            }
            return idLayer;
        }

        /// <summary>
        /// Проверка блокировки слоя IsOff IsLocked IsFrozen.
        /// Если заблокировано - то разблокируется.
        /// Если слоя нет - то он создается.
        /// </summary>
        /// <param name="layers">Список слоев для проверкм в текущей рабочей базе</param>
        public static Dictionary<string, ObjectId> CheckLayerState(this List<LayerInfo> layers)
        {
            Dictionary<string, ObjectId> resVal = new Dictionary<string, ObjectId>();
            Database db = HostApplicationServices.WorkingDatabase;
            using (var lt = db.LayerTableId.Open(OpenMode.ForRead) as LayerTable)
            {
                foreach (var layer in layers)
                {
                    if (lt.Has(layer.Name))
                    {                        
                        using (var lay = lt[layer.Name].Open(OpenMode.ForRead) as LayerTableRecord)
                        {
                            if (lay.IsLocked && lay.IsOff && lay.IsFrozen)
                            {
                                lay.UpgradeOpen();
                                if (lay.IsOff)
                                {
                                    lay.IsOff = false;
                                }
                                if (lay.IsLocked)
                                {
                                    lay.IsLocked = false;
                                }
                                if (lay.IsFrozen)
                                {
                                    lay.IsFrozen = false;
                                }
                            }
                            resVal.Add(layer.Name, lay.Id);
                        }
                    }
                    else
                    {
                        var layId = CreateLayer(layer, lt);
                        resVal.Add(layer.Name, layId);
                    }
                }
            }
            return resVal;
        }

        public static ObjectId CheckLayerState(this LayerInfo layer)
        {
            List<LayerInfo> layers = new List<LayerInfo>() { layer };
            var dictLays = CheckLayerState(layers);
            ObjectId res;
            dictLays.TryGetValue(layer.Name, out res);
            return res;
        }

        public static ObjectId CheckLayerState(string layer)
        {            
            LayerInfo li = new LayerInfo(layer);
            List<LayerInfo> layersInfo = new List<LayerInfo>();
            layersInfo.Add(li);
            var dictLays = CheckLayerState(layersInfo);
            ObjectId res;
            dictLays.TryGetValue(layer, out res);
            return res;
        }

        public static Dictionary<string, ObjectId> CheckLayerState(string[] layers)
        {
            List<LayerInfo> layersInfo = new List<LayerInfo>();
            foreach (var item in layers)
            {
                LayerInfo li = new LayerInfo(item);
                layersInfo.Add(li);
            }
            return CheckLayerState(layersInfo);
        }
    }
}