using AcadLib.XData;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace AcadLib
{
    /// <summary>
    /// Сохранение и извлечение значений из словаря чертежа
    /// Работает с HostApplicationServices.WorkingDatabase при каждом вызове метода сохранения или считывания значения.
    /// Или нужно задать Database в поле Db.
    /// </summary>
    public class DictNOD
    {
        private readonly string dictName;
        private readonly string dictInnerName;

        public Database Db { get; set; } = HostApplicationServices.WorkingDatabase;

        [Obsolete("Используй `innerDict` конструктор.")]
        public DictNOD(string dictName)
        {
            this.dictName = dictName;
        }

        // ReSharper disable once UnusedParameter.Local
        public DictNOD(string innerDict, bool hasInnerDict)
        {
            dictName = ExtDicHelper.PikApp;
            dictInnerName = innerDict;
        }

        /// <summary>
        /// Сохранение словаря
        /// </summary>
        /// <param name="dicEd">Словарь для сохранения</param>
        public void Save([CanBeNull] DicED dicEd)
        {
            if (string.IsNullOrEmpty(dicEd?.Name)) return;
            var dicId = GetDicPlugin(true);
            ExtDicHelper.SetDicED(dicId, dicEd);
        }

        /// <summary>
        /// Чтение вложенного словаря плагина - по имени вложенного словаря
        /// </summary>
        /// <param name="dicName">Имя словаря</param>
        /// <returns>Словарь по ключу `dicName` если он есть.</returns>
        [CanBeNull]
        public DicED LoadED(string dicName)
        {
            var dicPluginId = GetDicPlugin(false);
            var dicResId = ExtDicHelper.GetDic(dicPluginId, dicName, false, false);
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
        [CanBeNull]
        public DicED LoadED()
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
        [CanBeNull]
        [Obsolete("Используй `DicED`")]
        public List<TypedValue> Load(string recName)
        {
            List<TypedValue> values;
            var recId = GetRec(recName, false);
            if (recId.IsNull)
                return null;

            using (var xRec = recId.Open(OpenMode.ForRead) as Xrecord)
            {
                if (xRec == null) return null;
                using (var data = xRec.Data)
                {
                    if (data == null)
                        return null;
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
            var res = defValue; // default
            var idRec = GetRec(recName, false);
            if (idRec.IsNull)
                return res;

            using (var xRec = idRec.Open(OpenMode.ForRead) as Xrecord)
            {
                if (xRec == null) return res;
                using (var data = xRec.Data)
                {
                    if (data == null)
                        return res;
                    var values = data.AsArray();
                    if (values.Count() <= 0) return res;
                    try
                    {
                        return Convert.ToBoolean(values[0].Value);
                    }
                    catch
                    {
                        Logger.Log.Error(
                            $"Ошибка определения параметра '{recName}'='{values[0].Value}'. Взято значение по умолчанию '{defValue}'");
                        xRec.Close();
                        Save(defValue, recName);
                        res = defValue;
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
            var res = defaultValue;
            var idRec = GetRec(recName, false);
            if (idRec.IsNull)
                return res;

            using (var xRec = idRec.Open(OpenMode.ForRead) as Xrecord)
            {
                if (xRec == null) return res;
                using (var data = xRec.Data)
                {
                    if (data == null)
                        return res;
                    var values = data.AsArray();
                    if (values.Count() <= 0) return res;
                    try
                    {
                        return (int)values[0].Value;
                    }
                    catch
                    {
                        Logger.Log.Error(
                            $"Ошибка определения параметра '{recName}'='{values[0].Value}'. Взято значение по умолчанию '{defaultValue}'");
                        xRec.Close();
                        Save(defaultValue, recName);
                        res = defaultValue;
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
            var res = defaultValue;
            var idRec = GetRec(recName, false);
            if (idRec.IsNull)
                return res;

            using (var xRec = idRec.Open(OpenMode.ForRead) as Xrecord)
            {
                if (xRec == null) return res;
                using (var data = xRec.Data)
                {
                    if (data == null)
                        return res;
                    var values = data.AsArray();
                    if (values.Count() <= 0) return res;
                    try
                    {
                        return (double)values[0].Value;
                    }
                    catch
                    {
                        Logger.Log.Error(
                            $"Ошибка определения параметра '{recName}'='{values[0].Value}'. Взято значение по умолчанию '{defaultValue}'");
                        xRec.Close();
                        Save(defaultValue, recName);
                        res = defaultValue;
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
            var res = defaultValue;
            var idRec = GetRec(recName, false);
            if (idRec.IsNull)
                return res;

            using (var xRec = idRec.Open(OpenMode.ForRead) as Xrecord)
            {
                if (xRec == null) return res;
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
            var idRec = GetRec(key, true);
            if (idRec.IsNull)
                return;

            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                using (var rb = new ResultBuffer())
                {
                    rb.Add(new TypedValue((int)DxfCode.Bool, value));
                    if (xRec != null) xRec.Data = rb;
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
            var idRec = GetRec(keyName, true);
            if (idRec.IsNull)
                return;

            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                using (var rb = new ResultBuffer())
                {
                    rb.Add(new TypedValue((int)DxfCode.Int32, number));
                    if (xRec != null) xRec.Data = rb;
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
            var idRec = GetRec(keyName, true);
            if (idRec.IsNull)
                return;

            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                using (var rb = new ResultBuffer())
                {
                    rb.Add(new TypedValue((int)DxfCode.Real, number));
                    if (xRec != null) xRec.Data = rb;
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
            var idRec = GetRec(key, true);
            if (idRec.IsNull)
                return;

            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                using (var rb = new ResultBuffer())
                {
                    rb.Add(new TypedValue((int)DxfCode.Text, text));
                    if (xRec != null) xRec.Data = rb;
                }
            }
        }

        [Obsolete("Используй `DicED`")]
        public void Save([CanBeNull] List<TypedValue> values, string key)
        {
            if (values == null || values.Count == 0) return;
            var idRec = GetRec(key, true);
            if (idRec.IsNull)
                return;

            using (var xRec = idRec.Open(OpenMode.ForWrite) as Xrecord)
            {
                using (var rb = new ResultBuffer(values.ToArray()))
                {
                    if (xRec != null) xRec.Data = rb;
                }
            }
        }

        private ObjectId GetDicPlugin(bool create)
        {
            ObjectId res;
            var dicNodId = Db.NamedObjectsDictionaryId;
            var dicPikId = ExtDicHelper.GetDic(dicNodId, dictName, create, false);
            if (string.IsNullOrEmpty(dictInnerName))
            {
                res = dicPikId;
            }
            else
            {
                var dicPluginId = ExtDicHelper.GetDic(dicPikId, dictInnerName, create, false);
                res = dicPluginId;
            }
            return res;
        }

        [Obsolete("Используй `DicED`")]
        private ObjectId GetRec([NotNull] string key, bool create)
        {
            var dicId = GetDicPlugin(create);
            return ExtDicHelper.GetRec(dicId, key, create, false);
        }
    }
}