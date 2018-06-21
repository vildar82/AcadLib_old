// ReSharper disable once CheckNamespace
namespace AcadLib
{
    using System;
    using System.Collections.Generic;
    using Autodesk.AutoCAD.DatabaseServices;
    using JetBrains.Annotations;
    using XData;

    /// <summary>
    /// Расширенные данные объекта
    /// </summary>
    [PublicAPI]
    public static class XDataExt
    {
        private static readonly Dictionary<Type, int> dictXDataTypedValues = new Dictionary<Type, int>
        {
            { typeof(int), (int)DxfCode.ExtendedDataInteger32 },
            { typeof(double), (int)DxfCode.ExtendedDataReal },
            { typeof(string), (int)DxfCode.ExtendedDataAsciiString }
        };

        /// <summary>
        /// Регистрация приложения в RegAppTable
        /// </summary>
        public static void RegApp([NotNull] this Database db, string regAppName)
        {
            RegAppHelper.RegApp(db, regAppName);
        }

        /// <summary>
        /// Регистрация приложения PIK в RegAppTable
        /// </summary>
        /// <param name="db"></param>
        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static void RegAppPIK([NotNull] this Database db)
        {
            RegApp(db, ExtDicHelper.PikApp);
        }

        /// <summary>
        /// Удаление XData связанных с этим приложением
        /// </summary>
        /// <param name="dbo"></param>
        /// <param name="regAppName"></param>
        public static void RemoveXData([NotNull] this DBObject dbo, string regAppName)
        {
            if (dbo.GetXDataForApplication(regAppName) != null)
            {
                var rb = new ResultBuffer(new TypedValue(1001, regAppName));
                var isWriteEnabled = dbo.IsWriteEnabled;
                if (!isWriteEnabled)
                    dbo.UpgradeOpen();
                dbo.XData = rb;
                if (!isWriteEnabled)
                    dbo.DowngradeOpen();
            }
        }

        public static bool HasXData([NotNull] this DBObject dbo, [NotNull] string regApp)
        {
            return dbo.GetXDataForApplication(regApp) != null;
        }

        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static void RemoveXDataPIK([NotNull] this DBObject dbo)
        {
            RemoveXData(dbo, ExtDicHelper.PikApp);
        }

        /// <summary>
        /// Приложение не регистрируется !!!
        /// </summary>
        public static void SetXData([NotNull] this DBObject dbo, string regAppName, string value)
        {
            using (var rb = new ResultBuffer(
                new TypedValue((int)DxfCode.ExtendedDataRegAppName, regAppName),
                new TypedValue((int)DxfCode.ExtendedDataAsciiString, value)))
            {
                dbo.XData = rb;
            }
        }

        /// <summary>
        /// Приложение не регистрируется !!!
        /// </summary>
        public static void SetXData([NotNull] this DBObject dbo, string regAppName, int value)
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
        /// <param name="regAppName">Имя приложения</param>
        /// <param name="value">Значение одного из стандартного типа - int, double, string</param>
        public static void SetXData<T>([NotNull] this DBObject dbo, string regAppName, [NotNull] T value)
        {
            RegApp(dbo.Database, regAppName);
            var tvValu = GetTypedValue(value);
            using (var rb = new ResultBuffer(new TypedValue((int)DxfCode.ExtendedDataRegAppName, regAppName), tvValu))
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
        public static void SetXDataPIK<T>([NotNull] this DBObject dbo, [NotNull] T value)
        {
            SetXData(dbo, ExtDicHelper.PikApp, value);
        }

        /// <summary>
        /// Запись int
        /// ПРиложение не регистрируется
        /// </summary>
        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static void SetXDataPIK([NotNull] this DBObject dbo, int value)
        {
            SetXData(dbo, ExtDicHelper.PikApp, value);
        }

        public static int GetXData([NotNull] this DBObject dbo, string regAppName)
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
        public static int GetXDatPIK([NotNull] this DBObject dbo)
        {
            return GetXData(dbo, ExtDicHelper.PikApp);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static string GetXDataString([NotNull] this DBObject dbo, string regAppName)
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
        public static string GetXDatPIKString([NotNull] this DBObject dbo)
        {
            return GetXDataString(dbo, ExtDicHelper.PikApp);
        }

        /// <summary>
        /// Считывание значение с объекта
        /// </summary>
        /// <typeparam name="T">Тип значения - int, double, string</typeparam>
        /// <returns>Значение или дефолтное значение для этого типа (0,0,null) если не найдено</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        [CanBeNull]
        public static T GetXData<T>([NotNull] this DBObject dbo, string regAppName)
        {
            var rb = dbo.GetXDataForApplication(regAppName);
            if (rb != null)
            {
                var dxfT = dictXDataTypedValues[typeof(T)];
                foreach (var item in rb)
                {
                    if (item.TypeCode == dxfT)
                    {
                        return (T)item.Value;
                    }
                }
            }

            return default;
        }

        /// <summary>
        /// Считывание значения из XData определенного типа, приложения PIK
        /// </summary>
        /// <typeparam name="T">Тип значения - int, double, string</typeparam>
        /// <param name="dbo">Объект</param>
        /// <returns>Значение или дефолтное значение типа, если не найдено</returns>
        [Obsolete("Лучше использовать свой `regAppName` для каждого плагина (задачи)")]
        public static T GetXDataPIK<T>([NotNull] this DBObject dbo)
        {
            return GetXData<T>(dbo, ExtDicHelper.PikApp);
        }

        private static TypedValue GetTypedValue([NotNull] object value)
        {
            var dxfCode = dictXDataTypedValues[value.GetType()];
            var tv = new TypedValue(dxfCode, value);
            return tv;
        }
    }
}