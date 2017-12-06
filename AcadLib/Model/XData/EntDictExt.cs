using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcadLib.XData
{
    /// <summary>
    /// Расширенные данные примитива.    
    /// </summary>
    public class EntDictExt : IDisposable
    {

        readonly DBObject dbo;
        readonly string pluginName;

        public EntDictExt([NotNull] DBObject dbo, string pluginName)
        {
            if (dbo != null)
            {
                this.dbo = dbo;
                this.pluginName = pluginName;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Сохранение словаря в объект.
        /// dbo\ExtDic\Pik\Plugin\dic
        /// </summary>
        /// <param name="dic">Словарь для сохранения</param>
        public void Save(DicED dic)
        {
            var dicId = GetDicPlugin(true);
            ExtDicHelper.SetDicED(dicId, dic);
        }


        /// <summary>
        /// Чтение словаря плагина
        /// </summary>
        /// <returns>Словарь плагина. Имя DicED.Name - не заполняется.</returns>
        [CanBeNull]
        public DicED Load()
        {
            var dicId = GetDicPlugin(false);
            return ExtDicHelper.GetDicEd(dicId);
        }

        /// <summary>
        /// Удаление словаря из объекта
        /// </summary>
        public void Delete([CanBeNull] string dicName = null)
        {
            var dicId = GetDicPlugin(false);
            if (!string.IsNullOrEmpty(dicName))
            {
                // Проверить. Если в словаре объекта есть только удаляемый словарь по имени, то удалить весь словарь объекта
                var dicDbo = ExtDicHelper.GetDicEd(dicId);
                var dicDelete = dicDbo.GetInner(dicName);
                if (dicDelete != null)
                {
                    if (dicDbo.Inners.Count != 1 || dicDbo.Recs.Any())
                    {
                        //Удаление только словаря с этим именем
                        dicId = ExtDicHelper.GetDic(dicId, dicName, false, false);
                    }
                }
            }
            // Удаление словаря
            ExtDicHelper.DeleteDic(dicId, dbo);
        }

        /// <summary>
        /// Сохранение данных в объекте. 
        /// Можно передать только string, int, double
        /// </summary>            
        [Obsolete("Используй `DicED`")]
        public void Save(string rec, object value)
        {
            var values = new List<TypedValue> { TypedValueExt.GetTvExtData(value) };
            Save(values, rec);
        }

        [Obsolete("Используй `DicED`", true)]
        public void Save(List<TypedValue> values, string rec)
        {
            var idRec = GetXRecord(rec, true);
            if (idRec.IsNull) return;
            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                if (xRec == null) return;
                using (var rb = new ResultBuffer(values.ToArray()))
                {
                    xRec.Data = rb;
                }
            }
        }

        /// <summary>
        /// Чтение Xrecord из объекта
        /// </summary>
        /// <typeparam name="T">Хранимый тип - может быть string, int, double, List/<TypedValue/></typeparam>
        [Obsolete("Используй `DicED`")]
        public T Load<T>(string rec)
        {
            var res = default(T);
            var typeT = typeof(T);
            var type = GetExtendetDataType(typeT);

            var xRecId = GetXRecord(rec, false);
            if (!xRecId.IsNull)
            {
                using (var xRec = xRecId.Open(OpenMode.ForRead) as Xrecord)
                {
                    if (xRec != null)
                    {
                        foreach (var item in xRec.Data)
                        {
                            if (type == item.TypeCode)
                            {
                                return (T)item.Value;
                            }
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Чтение всех Xrecord из словаря плагина
        /// </summary>   
        [CanBeNull]
        [Obsolete("Используй `DicED`")]
        public Dictionary<string, List<TypedValue>> LoadAllXRecords()
        {
            Dictionary<string, List<TypedValue>> res = null;

            var dicPluginId = GetDicPlugin(false);
            if (!dicPluginId.IsNull)
            {
                using (var dicPlugin = dicPluginId.Open(OpenMode.ForRead) as DBDictionary)
                {
                    if (dicPlugin != null)
                    {
                        res = new Dictionary<string, List<TypedValue>>();
                        foreach (var item in dicPlugin)
                        {
                            var rec = item.Value.GetObject(OpenMode.ForRead) as Xrecord;
                            if (rec == null) continue;
                            res.Add(item.Key, rec.Data.AsArray().ToList());
                        }
                    }
                }
            }
            return res;
        }

        private ObjectId GetXRecord([NotNull] string key, bool create)
        {
            var res = ObjectId.Null;
            // Словарь плагина
            var idDicPlugin = GetDicPlugin(create);
            // Запись key
            var idRecKey = ExtDicHelper.GetRec(idDicPlugin, key, create, create);
            res = idRecKey;
            return res;
        }

        private ObjectId GetDicPlugin(bool create)
        {
            var res = ObjectId.Null;
            // Словарь объекта
            var idDboDic = ExtDicHelper.GetDboExtDic(dbo, create);
            // Словарь ПИК
            var idDicPik = ExtDicHelper.GetDic(idDboDic, ExtDicHelper.PikApp, create, false);
            // Словарь плагина
            var idDicPlugin = ExtDicHelper.GetDic(idDicPik, pluginName, create, false);
            res = idDicPlugin;
            return res;
        }

        [Obsolete("Используй `TypedValueExt`")]
        private int GetExtendetDataType([NotNull] Type value)
        {
            if (value == typeof(string))
            {
                return (int)DxfCode.ExtendedDataAsciiString;
            }
            else if (value == typeof(int))
            {
                return (int)DxfCode.ExtendedDataInteger32;
            }
            else if (value == typeof(double))
            {
                return (int)DxfCode.ExtendedDataReal;
            }
            else
            {
                throw new ArgumentException($"В расшир.данные можно сохранять только string, int, double, а тип '{value.GetType()}' нет.");
            }
        }

        public void Dispose()
        {
            if (dbo != null && !dbo.IsDisposed)
            {
                dbo.Dispose();
            }
        }
    }
}