using System;
using System.Collections.Generic;
using AcadLib.XData;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    /// <summary>
    /// Расширенные данные объекта
    /// </summary>
    public static class XDataExt
    {
        private static readonly Dictionary<Type, int> dictXDataTypedValues = new Dictionary<Type, int> {            
            { typeof(int), (int)DxfCode.ExtendedDataInteger32 },
            { typeof(double), (int)DxfCode.ExtendedDataReal },
            { typeof(string),(int)DxfCode.ExtendedDataAsciiString }
        };

        /// <summary>
        /// Регистрация приложения в RegAppTable
        /// </summary>        
        public static void RegApp(this Database db, string regAppName)
        {
            RegAppHelper.RegApp(db, regAppName);
        }

        /// <summary>
        /// Регистрация приложения PIK в RegAppTable
        /// </summary>
        /// <param name="db"></param>
        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static void RegAppPIK(this Database db)
        {
            RegApp(db, ExtDicHelper.PikApp);
        }

        /// <summary>
        /// Удаление XData связанных с этим приложением
        /// </summary>
        /// <param name="dbo"></param>
        /// <param name="regAppName"></param>
        public static void RemoveXData(this DBObject dbo, string regAppName)
        {
            if (dbo.GetXDataForApplication(regAppName) != null)
            {
                var rb = new ResultBuffer(new TypedValue(1001, regAppName));
                dbo.UpgradeOpen();
                dbo.XData = rb;
                dbo.DowngradeOpen();
            }
        }

        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static void RemoveXDataPIK(this DBObject dbo)
        {
            RemoveXData(dbo, ExtDicHelper.PikApp);
        }

        /// <summary>
        /// Приложение не регистрируется !!!
        /// </summary>        
        public static void SetXData(this DBObject dbo, string regAppName, int value)
        {
            using (var rb = new ResultBuffer(
                        new TypedValue((int)DxfCode.ExtendedDataRegAppName, regAppName),
                        new TypedValue((int)DxfCode.ExtendedDataInteger32, value)))
            {
                dbo.XData = rb;
            }
        }

        /// <summary>
        /// Запись значения в XData
        /// Регистрируется приложение regAppName
        /// </summary>
        /// <param name="dbo">DBObject</param>
        /// <param name="value">Значение одного из стандартного типа - int, double, string</param>
        public static void SetXData<T> (this DBObject dbo, string regAppName, T value)
        {
            RegApp(dbo.Database, regAppName);
            var tvValu = GetTypedValue(value);
            using (var rb = new ResultBuffer(
                        new TypedValue((int)DxfCode.ExtendedDataRegAppName, ExtDicHelper.PikApp),
                        tvValu))
            {
                dbo.XData = rb;
            }
        }

        /// <summary>
        /// Запись значения в XData
        /// Регистрируется приложение regAppName
        /// </summary>
        /// <param name="dbo">DBObject</param>
        /// <param name="value">Значение одного из стандартного типа - int, double, string</param>
        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static void SetXDataPIK<T> (this DBObject dbo, T value)
        {
            SetXData(dbo, ExtDicHelper.PikApp, value);
        }

        /// <summary>
        /// Запись int
        /// ПРиложение не регистрируется   
        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static void SetXDataPIK(this DBObject dbo, int value)
        {
            SetXData(dbo, ExtDicHelper.PikApp, value);
        }
        
        public static int GetXData(this DBObject dbo, string regAppName)
        {
            var rb = dbo.GetXDataForApplication(regAppName);
            if (rb != null)
            {
                foreach (var item in rb)
                {
                    if (item.TypeCode == (short)DxfCode.ExtendedDataInteger32)
                    {
                        return (int)item.Value;
                    }
                }
            }
            return 0;
        }
        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static int GetXDatPIK(this DBObject dbo)
        {
            return GetXData(dbo, ExtDicHelper.PikApp);
        }

        public static string GetXDataString(this DBObject dbo, string regAppName)
        {
            var rb = dbo.GetXDataForApplication(regAppName);
            if (rb != null)
            {
                foreach (var item in rb)
                {
                    if (item.TypeCode == (short)DxfCode.ExtendedDataAsciiString)
                    {
                        return (string)item.Value;
                    }
                }
            }
            return string.Empty;
        }
        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static string GetXDatPIKString(this DBObject dbo)
        {
            return GetXDataString(dbo, ExtDicHelper.PikApp);
        }

        /// <summary>
        /// Считывание значение с объекта
        /// </summary>
        /// <typeparam name="T">Тип значения - int, double, string</typeparam>        
        /// <returns>Значение или дефолтное значение для этого типа (0,0,null) если не найдено</returns>
        public static T GetXData<T> (this DBObject dbo, string regAppName)
        {
            var dxfT = dictXDataTypedValues[typeof(T)];
            var rb = dbo.GetXDataForApplication(regAppName);
            if (rb != null)
            {
                foreach (var item in rb)
                {
                    if (item.TypeCode == dxfT)
                    {
                        return (T)item.Value;
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// Считывание значения из XData определенного типа, приложения PIK
        /// </summary>
        /// <typeparam name="T">Тип значения - int, double, string</typeparam>
        /// <param name="dbo">Объект</param>
        /// <returns>Значение или дефолтное значение типа, если не найдено</returns>
        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static T GetXDataPIK<T> (this DBObject dbo)
        {
            return GetXData<T>(dbo, ExtDicHelper.PikApp);
        }

        private static TypedValue GetTypedValue (object value)
        {
            var dxfCode = dictXDataTypedValues[value.GetType()];
            var tv = new TypedValue(dxfCode, value);
            return tv;
        }
    }
}
