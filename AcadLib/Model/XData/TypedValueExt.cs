using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    public static class TypedValueExt
    {
        /// <summary>
        /// Возвращает значение TypedValue
        /// Типы - int, double, string - те которые возможны в TypedValue DxfCode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tv"></param>
        /// <returns></returns>
        public static T GetTvValue<T> (this TypedValue tv)
        {
            T res;
            if (tv.Value is T)
            {                
                res = (T)tv.Value;
            }
            else
            {
                res = default(T);
            }
            return res;
        }

        /// <summary>
        /// Создание TypedValue для сохранение в расширенные данные DxfCode.ExtendedData
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TypedValue GetTvExtData(object value)
        {
            var typeObj = value.GetType();

            int code = 0;
            object tvValue = value;

            if (typeObj == typeof(bool))
            {
                code = (int)DxfCode.ExtendedDataInteger32;
                tvValue = (bool)value ? 1 : 0;
            }
            else if (typeObj == typeof (int))
            {
                code = (int)DxfCode.ExtendedDataInteger32;
            }
            else if (typeObj == typeof(byte))
            {
                code = (int)DxfCode.ExtendedDataInteger32;
            }
            else if (typeObj == typeof(double))
            {
                code = (int)DxfCode.ExtendedDataReal;
            }
            else if (typeObj == typeof(string))
            {
                code = (int)DxfCode.ExtendedDataAsciiString;                
            }

            if (code == 0)
                throw new Exception("Invalid TypedValue");

            return new TypedValue(code, tvValue);
        }
    }
}
