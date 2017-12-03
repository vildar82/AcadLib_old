using System;
using System.Collections.Generic;
using AcadLib.Colors;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using NetLib;

namespace AcadLib
{
    public static class TypedValueExt
    {        
        public static T GetValue<T>(this Dictionary<string, object> dictValues, string name, T defaultValue)
        {
            if (dictValues == null) return defaultValue;
            if (dictValues.TryGetValue(name, out var value))
            {
                try
                {
                    return value.GetValue<T>();
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, $"TypedValueExt, GetValue из словаря значений - по имени параметра '{name}'. object = {value.ToString()} тип {value.GetType()}");
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        public static Dictionary<string, object> ToDictionary(this IEnumerable<TypedValue> values)
        {            
            var dictValues = new Dictionary<string, object>();
            if (values == null) return dictValues;
            var name = string.Empty;            
            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    try
                    {
                        dictValues.Add(name, item.Value);
                        name = string.Empty;
                    }
                    catch(Exception ex)
                    {
                        Logger.Log.Error(ex, $"ToDictionary() - name={name}, value={item.Value}");
                    }
                }
                else
                {
                    name = item.GetTvValue<string>();
                }
            }
            return dictValues;
        }

        /// <summary>
        /// Возвращает значение TypedValue
        /// Типы - int, bool, byte, double, string - те которые возможны в TypedValue DxfCode
        /// Не проверяется соответствие типа значения и номера кода DxfCode !!!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tv"></param>
        /// <returns></returns>
        public static T GetTvValue<T> (this TypedValue tv)
        {
            T res;
            try
            {
                return tv.Value.GetValue<T>();                
            }
            catch
            {
                res = default;
            }
            return res;
        }

        /// <summary>
        /// Создание TypedValue для сохранение в расширенные данные DxfCode.ExtendedData
        /// bool, byte, int, double, string
        /// </summary>        
        public static TypedValue GetTvExtData(object value)
        {
            if (value == null) return new TypedValue();
            var typeObj = value.GetType();

            var code = 0;
            var tvValue = value;

            switch (value)
            {
                case bool b:
                    code = (int)DxfCode.ExtendedDataInteger32;
                    tvValue = b ? 1 : 0;
                    break;
                case Enum en:
                    code = (int)DxfCode.ExtendedDataInteger32;
                    tvValue = (int)value;
                    break;
                case int i:
                    code = (int)DxfCode.ExtendedDataInteger32;
                    break;
                case byte b:
                    code = (int)DxfCode.ExtendedDataInteger32;
                    break;
                case double d:
                    code = (int)DxfCode.ExtendedDataReal;
                    break;
                case string s:
                    code = (int)DxfCode.ExtendedDataAsciiString;
                    break;
            }

            if (code == 0)
                throw new Exception("Invalid TypedValue");

            return new TypedValue(code, tvValue);
        }
    }
}
