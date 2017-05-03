using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Layers
{
    public static class LayerExt
    {
        static string groupLayerPrefix;

        /// <summary>
        /// Префикс слоев - группа пользователя
        /// </summary>
        public static string GroupLayerPrefix
        {
            get {
                if (groupLayerPrefix == null)
                {
                    groupLayerPrefix = GetGroupLayerPrefix();
                }
                return groupLayerPrefix;
            }
        }

        /// <summary>
        /// Получение слоя.
        /// Если его нет в базе, то создается.      
        /// </summary>
        /// <param name="layerInfo">параметры слоя</param>
        /// <returns></returns>
        public static ObjectId GetLayerOrCreateNew(this LayerInfo layerInfo)
        {
            var idLayer = ObjectId.Null;
            var db = HostApplicationServices.WorkingDatabase;
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
            var idLayer = ObjectId.Null;
            // Если слоя нет, то он создается.            
            using (var newLayer = new LayerTableRecord())
            {
                layerInfo.SetProp(newLayer, lt.Database);
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
        public static Dictionary<string, ObjectId> CheckLayerState(this List<LayerInfo> layers, bool checkProps)
        {
            var resVal = new Dictionary<string, ObjectId>();
            var db = HostApplicationServices.WorkingDatabase;
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
                            if (checkProps)
                            {
                                layer.SetProp(lay, db);
                            }
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

        /// <summary>
        /// Проверка блокировки слоя IsOff IsLocked IsFrozen.
        /// Если заблокировано - то разблокируется.
        /// Если слоя нет - то он создается.
        /// </summary>
        /// <param name="layers">Список слоев для проверкм в текущей рабочей базе</param>
        public static Dictionary<string, ObjectId> CheckLayerState(this List<LayerInfo> layers)
        {
            return CheckLayerState(layers, false);
        }

        public static ObjectId CheckLayerState(this LayerInfo layer, bool checkProps)
        {
            var layers = new List<LayerInfo>() { layer };
            var dictLays = CheckLayerState(layers, checkProps);
            ObjectId res;
            dictLays.TryGetValue(layer.Name, out res);
            return res;
        }
        public static ObjectId CheckLayerState(this LayerInfo layer)
        {
            return CheckLayerState(layer, false);
        }

        public static ObjectId CheckLayerState(string layer)
        {            
            var li = new LayerInfo(layer);
            var layersInfo = new List<LayerInfo>();
            layersInfo.Add(li);
            var dictLays = CheckLayerState(layersInfo);
            ObjectId res;
            dictLays.TryGetValue(layer, out res);
            return res;
        }

        public static Dictionary<string, ObjectId> CheckLayerState(string[] layers)
        {
            var layersInfo = new List<LayerInfo>();
            foreach (var item in layers)
            {
                var li = new LayerInfo(item);
                layersInfo.Add(li);
            }
            return CheckLayerState(layersInfo);
        }

        private static string GetGroupLayerPrefix()
        {
            var usergroup = AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup;
            if (usergroup == "КР-МН")
            {
                return "КР";
            }
            if (usergroup == "КР-СБ")
            {
                return "СБ";
            }
            if (usergroup == "КР-СБ-ГК")
            {
                return string.Empty;
            }
            if (usergroup.StartsWith("ГП"))
            {
                return "ГП";
            }
            if (usergroup.Contains(","))
            {
                return string.Empty;
            }
            if (usergroup == "ЖБК-ТО")
            {
                return "ОЗЖБК";
            }
            return usergroup;
        }
    }
}