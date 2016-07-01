using System;
using System.Collections.Generic;
using System.Linq;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    /// <summary>
    /// Сохранение и извлечение значений из словаря чертежа
    /// Работает с HostApplicationServices.WorkingDatabase при каждом вызове метода сохранения или считывания значения.
    /// </summary>
    public class DictNOD
    {
        private string dictName;
        private string dictInnerName;

        [Obsolete("Используй innerDict конструктор.")]
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
        /// Считывание из словаря, текущей рабочей бд, булевого значения из первой записи TypedValue в xRecord заданного имени.
        /// </summary>
        /// <param name="recName">Имя XRecord записи в словаре</param>
        /// <param name="defValue">Возвращаемое значение если такой записи нет в словаре.</param>
        /// <returns></returns>
        public bool Load(string recName, bool defValue)
        {
            bool res = defValue; // default
            ObjectId idRec = getRec(recName, false);
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
                            Inspector.AddError($"Ошибка определения параметра '{recName}'='{values[0].Value}'. Взято значение по умолчанию '{defValue}'");
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
        public int Load(string recName, int defaultValue)
        {
            int res = defaultValue;
            ObjectId idRec = getRec(recName, false);
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
                            Inspector.AddError($"Ошибка определения параметра '{recName}'='{values[0].Value}'. Взято значение по умолчанию '{defaultValue}'");
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
        public double Load(string recName, double defaultValue)
        {
            double res = defaultValue;
            ObjectId idRec = getRec(recName, false);
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
                            Inspector.AddError($"Ошибка определения параметра '{recName}'='{values[0].Value}'. Взято значение по умолчанию '{defaultValue}'");
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
        /// <returns></returns>
        public string Load(string recName, string defaultValue)
        {
            string res = defaultValue;
            ObjectId idRec = getRec(recName, false);
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
        public void Save(bool value, string key)
        {
            ObjectId idRec = getRec(key, true);
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
        public void Save(int number, string keyName)
        {
            ObjectId idRec = getRec(keyName, true);
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
        public void Save(double number, string keyName)
        {
            ObjectId idRec = getRec(keyName, true);
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
        public void Save(string text, string key)
        {
            ObjectId idRec = getRec(key, true);
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

        private ObjectId getDict(bool create)
        {
            ObjectId idDic = ObjectId.Null;
            Database db = HostApplicationServices.WorkingDatabase;

            using (DBDictionary nod = (DBDictionary)db.NamedObjectsDictionaryId.Open(OpenMode.ForRead))
            {
                if (!nod.Contains(dictName))
                {
                    if (!create) return idDic;
                    nod.UpgradeOpen();
                    using (var dic = new DBDictionary())
                    {
                        idDic = nod.SetAt(dictName, dic);
                        dic.TreatElementsAsHard = true;
                        if (!string.IsNullOrEmpty(dictInnerName))
                        {
                            using (var dicInner = new DBDictionary())
                            {
                                idDic = dic.SetAt(dictInnerName, dicInner);
                                dicInner.TreatElementsAsHard = true;
                            }
                        }
                    }
                }
                else
                {
                    idDic = nod.GetAt(dictName);
                    if (!string.IsNullOrEmpty(dictInnerName))
                    {
                        using (var dic = idDic.Open(OpenMode.ForRead) as DBDictionary)
                        {                   
                            if (dic.Contains(dictInnerName))
                            {
                                idDic = dic.GetAt(dictInnerName);
                            }
                            else
                            {
                                if (create)
                                {
                                    using (var dicInner = new DBDictionary())
                                    {
                                        dic.UpgradeOpen();
                                        idDic = dic.SetAt(dictInnerName, dicInner);
                                        dicInner.TreatElementsAsHard = true;
                                    }
                                }
                                else
                                {
                                    idDic = ObjectId.Null;
                                }
                            }
                        }
                    }
                }
            }
            return idDic;
        }

        private ObjectId getRec(string recName, bool create)
        {
            ObjectId idDict = getDict(create);
            if (idDict.IsNull)
            {
                return ObjectId.Null;
            }
            ObjectId idRec = ObjectId.Null;
            using (var dic = idDict.Open(OpenMode.ForRead) as DBDictionary)
            {
                if (!dic.Contains(recName))
                {
                    using (var xRec = new Xrecord())
                    {
                        dic.UpgradeOpen();
                        idRec = dic.SetAt(recName, xRec);
                    }
                }
                else idRec = dic.GetAt(recName);
            }
            return idRec;
        }
    }
}