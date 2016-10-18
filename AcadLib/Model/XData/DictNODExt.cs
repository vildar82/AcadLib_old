using System;
using System.Collections.Generic;
using System.Linq;
using AcadLib.Errors;
using AcadLib.XData;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    /// <summary>
    /// Сохранение и извлечение значений из словаря чертежа
    /// Работает с HostApplicationServices.WorkingDatabase при каждом вызове метода сохранения или считывания значения.
    /// Или нужно задать Database в поле Db.
    /// </summary>
    public class DictNOD
    {
        private string dictName;
        private string dictInnerName;

        public Database Db { get; set; } = HostApplicationServices.WorkingDatabase;

        [Obsolete("Используй `innerDict` конструктор.")]
        public DictNOD(string dictName)
        {
            this.dictName = dictName;
        }

        public DictNOD(string innerDict, bool hasInnerDict)
        {
            this.dictName = "PIK";
            this.dictInnerName = innerDict;            
        }

        /// <summary>
        /// Сохранение словаря
        /// </summary>
        /// <param name="dicEd">Словарь для сохранения</param>
        public void Save (DicED dicEd)
        {
            if (dicEd == null || string.IsNullOrEmpty(dicEd.Name)) return;
            var dicId = GetDicPlugin(true);
            ExtDicHelper.SetDicED(dicId, dicEd);
        }

        /// <summary>
        /// Чтение вложенного словаря плагина - по имени вложенного словаря
        /// </summary>
        /// <param name="dicName">Имя словаря</param>
        /// <returns>Словарь по ключу `dicName` если он есть.</returns>
        public DicED LoadED (string dicName)
        {
            var dicPluginId = GetDicPlugin(false);
            var dicResId = ExtDicHelper.GetDic(dicPluginId, dicName, false);
            var res = ExtDicHelper.GetDicEd(dicResId);
            if (res != null)
            {
                res.Name = dicName;
            }
            return res;
        }

        /// <summary>
        /// Чтение всего словаря плагина
        /// </summary>        
        public DicED LoadED ()
        {
            var dicPluginId = GetDicPlugin(false);            
            var res = ExtDicHelper.GetDicEd(dicPluginId);
            if (res != null)
            {
                res.Name = dictInnerName;
            }
            return res;
        }

        /// <summary>
        /// Чтение списка записей для заданной XRecord по имени
        /// </summary>
        /// <param name="recName">Имя XRecord в словаре</param>
        /// <returns>Список значений в XRecord или null</returns>
        [Obsolete("Используй `DicED`")]
        public List<TypedValue> Load (string recName)
        {
            List<TypedValue> values = null;

            var recId = GetRec(recName, false);           
            if (recId.IsNull)
                return values;

            using (var xRec = recId.Open(OpenMode.ForRead) as Xrecord)
            {
                using (var data = xRec.Data)
                {
                    if (data == null)
                        return values;
                    values = data.AsArray().ToList();                    
                }
            }
            return values;
        }

        /// <summary>
        /// Считывание из словаря, текущей рабочей бд, булевого значения из первой записи TypedValue в xRecord заданного имени.
        /// </summary>
        /// <param name="recName">Имя XRecord записи в словаре</param>
        /// <param name="defValue">Возвращаемое значение если такой записи нет в словаре.</param>
        /// <returns></returns>
        [Obsolete("Используй `DicED`")]
        public bool Load(string recName, bool defValue)
        {
            bool res = defValue; // default
            ObjectId idRec = GetRec(recName, false);
            if (idRec.IsNull)
                return res;

            using (var xRec = idRec.Open(OpenMode.ForRead) as Xrecord)
            {
                using (var data = xRec.Data)
                {
                    if (data == null)
                        return res;
                    var values = data.AsArray();
                    if (values.Count() > 0)
                    {
                        try
                        {
                            return Convert.ToBoolean(values[0].Value);
                        }
                        catch
                        {
                            Logger.Log.Error($"Ошибка определения параметра '{recName}'='{values[0].Value}'. Взято значение по умолчанию '{defValue}'");
                            xRec.Close();
                            Save(defValue, recName);
                            res = defValue;
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Считывание из словаря, текущей рабочей бд, целого значения из первой записи TypedValue в xRecord заданного имени.
        /// </summary>
        /// <param name="recName">Имя XRecord записи в словаре</param>
        /// <param name="defaultValue">Возвращаемое значение если такой записи нет в словаре.</param>
        /// <returns></returns>
        [Obsolete("Используй `DicED`")]
        public int Load(string recName, int defaultValue)
        {
            int res = defaultValue;
            ObjectId idRec = GetRec(recName, false);
            if (idRec.IsNull)
                return res;

            using (var xRec = idRec.Open(OpenMode.ForRead) as Xrecord)
            {
                using (var data = xRec.Data)
                {
                    if (data == null)
                        return res;
                    var values = data.AsArray();
                    if (values.Count() > 0)
                    {                        
                        try
                        {
                            return (int)values[0].Value;
                        }
                        catch
                        {
                            Logger.Log.Error($"Ошибка определения параметра '{recName}'='{values[0].Value}'. Взято значение по умолчанию '{defaultValue}'");
                            xRec.Close();
                            Save(defaultValue, recName);
                            res = defaultValue;
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Считывание из словаря, текущей рабочей бд, десятичного значения из первой записи TypedValue в xRecord заданного имени.
        /// </summary>
        /// <param name="recName">Имя XRecord записи в словаре</param>
        /// <param name="defaultValue">Возвращаемое значение если такой записи нет в словаре.</param>
        /// <returns></returns>
        [Obsolete("Используй `DicED`")]
        public double Load(string recName, double defaultValue)
        {
            double res = defaultValue;
            ObjectId idRec = GetRec(recName, false);
            if (idRec.IsNull)
                return res;

            using (var xRec = idRec.Open(OpenMode.ForRead) as Xrecord)
            {
                using (var data = xRec.Data)
                {
                    if (data == null)
                        return res;
                    var values = data.AsArray();
                    if (values.Count() > 0)
                    {                        
                        try
                        {
                            return (double)values[0].Value;
                        }
                        catch
                        {
                            Logger.Log.Error($"Ошибка определения параметра '{recName}'='{values[0].Value}'. Взято значение по умолчанию '{defaultValue}'");
                            xRec.Close();
                            Save(defaultValue, recName);
                            res = defaultValue;
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Считывание из словаря, текущей рабочей бд, строки значения из первой записи TypedValue в xRecord заданного имени.
        /// </summary>
        /// <param name="recName">Имя XRecord записи в словаре</param>
        /// <param name="defaultValue">Возвращаемое значение если такой записи нет в словаре.</param>
        ///<returns></returns>
        [Obsolete("Используй `DicED`")]
        public string Load(string recName, string defaultValue)
        {
            string res = defaultValue;
            ObjectId idRec = GetRec(recName, false);
            if (idRec.IsNull)
                return res;

            using (var xRec = idRec.Open(OpenMode.ForRead) as Xrecord)
            {
                using (var data = xRec.Data)
                {
                    if (data == null)
                        return res;
                    var values = data.AsArray();
                    if (values.Count() > 0)
                    {
                        return values[0].Value.ToString();
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Сохранение булевого значения в словарь
        /// </summary>
        /// <param name="value">Сохраняемое значение</param>
        /// <param name="key">Имя записи XRecord с одним TypedValue</param>
        [Obsolete("Используй `DicED`")]
        public void Save(bool value, string key)
        {
            ObjectId idRec = GetRec(key, true);
            if (idRec.IsNull)
                return;

            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                using (ResultBuffer rb = new ResultBuffer())
                {
                    rb.Add(new TypedValue((int)DxfCode.Bool, value));
                    xRec.Data = rb;
                }
            }
        }

        /// <summary>
        /// Сохранение целого значения в словарь
        /// </summary>
        /// <param name="number">Значение</param>
        /// <param name="keyName">Имя записи XRecord с одним TypedValue</param>
        [Obsolete("Используй `DicED`")]
        public void Save(int number, string keyName)
        {
            ObjectId idRec = GetRec(keyName, true);
            if (idRec.IsNull)
                return;

            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                using (ResultBuffer rb = new ResultBuffer())
                {
                    rb.Add(new TypedValue((int)DxfCode.Int32, number));
                    xRec.Data = rb;
                }
            }
        }

        /// <summary>
        /// Сохранение десятичного значения в словарь
        /// </summary>
        /// <param name="number">Значение</param>
        /// <param name="keyName">Имя записи XRecord с одним TypedValue</param>
        [Obsolete("Используй `DicED`")]
        public void Save(double number, string keyName)
        {
            ObjectId idRec = GetRec(keyName, true);
            if (idRec.IsNull)
                return;

            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                using (ResultBuffer rb = new ResultBuffer())
                {
                    rb.Add(new TypedValue((int)DxfCode.Real, number));
                    xRec.Data = rb;
                }
            }
        }

        /// <summary>
        /// Сохранение строки значения в словарь
        /// </summary>
        /// <param name="text">Значение</param>
        /// <param name="key">Имя записи XRecord с одним TypedValue</param>
        [Obsolete("Используй `DicED`")]
        public void Save(string text, string key)
        {
            ObjectId idRec = GetRec(key, true);
            if (idRec.IsNull)
                return;

            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                using (ResultBuffer rb = new ResultBuffer())
                {
                    rb.Add(new TypedValue((int)DxfCode.Text, text));
                    xRec.Data = rb;
                }
            }
        }

        [Obsolete("Используй `DicED`")]
        public void Save (List<TypedValue> values, string key)
        {
            if (values == null || values.Count == 0) return;
            ObjectId idRec = GetRec(key, true);
            if (idRec.IsNull)
                return;

            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                using (ResultBuffer rb = new ResultBuffer(values.ToArray()))
                {
                    xRec.Data = rb;                    
                }
            }
        }

        private ObjectId GetDicPlugin (bool create)
        {
            ObjectId res = ObjectId.Null;

            var dicNodId = Db.NamedObjectsDictionaryId;
            var dicPikId = ExtDicHelper.GetDic(dicNodId, dictName, create);
            if (string.IsNullOrEmpty( dictInnerName))
            {
                res = dicPikId;
            }
            else
            {
                var dicPluginId = ExtDicHelper.GetDic(dicPikId, dictInnerName, create);
                res = dicPluginId;
            }
            return res;
        }

        [Obsolete("Используй `DicED`")]
        private ObjectId GetRec (string key, bool create)
        {
            var dicId = GetDicPlugin(create);
            return ExtDicHelper.GetRec(dicId, key, create);            
        }
    }
}