using System;
using System.Collections.Generic;
using System.Linq;
using AutoCAD_PIK_Manager.Settings;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using JetBrains.Annotations;
using NetLib;

namespace AcadLib
{
    [PublicAPI]
    public static class General
    {
        /// <summary>
        /// Отменено пользователем.
        /// Сообщение для исключения при отмене команды пользователем.
        /// </summary>
        [Obsolete]
        public const string CanceledByUser = "Отменено пользователем";

        public const string Company = AutoCAD_PIK_Manager.CompanyInfo.NameEngShort;
        public const string UserGroupAR = "АР";
        public const string UserGroupEO = "ЭО";
        public const string UserGroupGBKTO = "ЖБК-ТО";
        public const string UserGroupGP = "ГП";
        public const string UserGroupGPTest = "ГП_Тест";
        public const string UserGroupKRMN = "КР-МН";
        public const string UserGroupKRSB = "КР-СБ";
        public const string UserGroupKRSBGK = "КР-СБ-ГК";
        public const string UserGroupOV = "ОВ";
        public const string UserGroupSS = "СС";
        public const string UserGroupVK = "ВК";
        public static readonly RXClass ClassAttDef = RXObject.GetClass(typeof(AttributeDefinition));
        public static readonly RXClass ClassBlRef = RXObject.GetClass(typeof(BlockReference));
        public static readonly RXClass ClassDBDic = RXObject.GetClass(typeof(DBDictionary));
        public static readonly RXClass ClassDbTextRX = RXObject.GetClass(typeof(DBText));
        public static readonly RXClass ClassDimension = RXObject.GetClass(typeof(Dimension));
        public static readonly RXClass ClassHatch = RXObject.GetClass(typeof(Hatch));
        public static readonly RXClass ClassMLeaderRX = RXObject.GetClass(typeof(MLeader));
        public static readonly RXClass ClassMTextRX = RXObject.GetClass(typeof(MText));
        public static readonly RXClass ClassPolyline = RXObject.GetClass(typeof(Polyline));
        public static readonly RXClass ClassRecord = RXObject.GetClass(typeof(Xrecord));
        public static readonly RXClass ClassRegion = RXObject.GetClass(typeof(Region));
        public static readonly RXClass ClassVport = RXObject.GetClass(typeof(Viewport));
        private static readonly List<string> bimUsers = new List<string>
        {
            "PrudnikovVS", "vrublevskiyba", "khisyametdinovvt", "arslanovti", "karadzhayanra"
        };

        /// <summary>
        /// BIM-manager - отдел поддержки пользователей
        /// </summary>
        public static bool IsBimUser { get; }

        static General()
        {
            IsBimUser = bimUsers.Any(u => u.EqualsIgnoreCase(Environment.UserName));
        }

        /// <summary>
        /// Файл из папки пользовательских данных (AppData\PIK\Autocad\...)
        /// </summary>
        /// <param name="pluginName">Имя программы(плагина)</param>
        /// <param name="fileName">Имя файла</param>
        [NotNull]
        [Obsolete("Используй Path.GetUserPluginFolder()")]
        public static string GetUserDataFile([NotNull] string pluginName, [NotNull] string fileName)
        {
            return IO.Path.GetUserPluginFile(pluginName, fileName);
        }

        public static bool IsCadManager()
        {
            return Environment.UserName.Equals(PikSettings.PikFileSettings.LoginCADManager, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Символы строковые
        /// </summary>
        [PublicAPI]
        public static class Symbols
        {
            /// <summary>
            /// Кубическая степень- ³
            /// </summary>
            public const string Cubic = "³";

            /// <summary>
            /// Градус - °
            /// </summary>
            public const string Degree = "°";

            /// <summary>
            /// Диаметр ⌀
            /// </summary>
            public const string Diam = "⌀";

            /// <summary>
            /// Квадратная степень- ²
            /// </summary>
            public const string Square = "²";
        }
    }
}