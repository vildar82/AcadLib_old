using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoCAD_PIK_Manager.Settings;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using NetLib;

namespace AcadLib
{
    public static class General
    {
		private static readonly List<string> bimUsers = new List<string>{ "PrudnikovVS", "vrublevskiyba", "khisyametdinovvt", "arslanovti" };
        public const string Company = AutoCAD_PIK_Manager.CompanyInfo.NameEngShort;

        public const string UserGroupAR = "АР";
        public const string UserGroupKRMN = "КР-МН";
        public const string UserGroupKRSB = "КР-СБ";
        public const string UserGroupKRSBGK = "КР-СБ-ГК";
        public const string UserGroupVK = "ВК";
        public const string UserGroupOV = "ОВ";
        public const string UserGroupSS = "СС";
        public const string UserGroupEO = "ЭО";
        public const string UserGroupGBKTO = "ЖБК-ТО";
        public const string UserGroupGP = "ГП";
        public const string UserGroupGPTest = "ГП_Тест";

	    public static readonly RXClass ClassAttDef = RXObject.GetClass(typeof(AttributeDefinition));
	    public static readonly RXClass ClassBlRef = RXObject.GetClass(typeof(BlockReference));
		public static readonly RXClass ClassDbTextRX = RXObject.GetClass(typeof(DBText));
	    public static readonly RXClass ClassMTextRX = RXObject.GetClass(typeof(MText));
	    public static readonly RXClass ClassMLeaderRX = RXObject.GetClass(typeof(MLeader));
	    public static readonly RXClass ClassDimension = RXObject.GetClass(typeof(Dimension));
	    public static readonly RXClass ClassRecord = RXObject.GetClass(typeof(Xrecord));
	    public static readonly RXClass ClassDBDic = RXObject.GetClass(typeof(DBDictionary));
	    public static readonly RXClass ClassPolyline = RXObject.GetClass(typeof(Polyline));
        public static readonly RXClass ClassHatch = RXObject.GetClass(typeof(Hatch));
        public static readonly RXClass ClassVport = RXObject.GetClass(typeof(Viewport));
        public static readonly RXClass ClassRegion = RXObject.GetClass(typeof(Region));

        static General()
	    {
		    IsBimUser = bimUsers.Any(u => u.EqualsIgnoreCase(Environment.UserName));
		}

	    public static bool IsBimUser { get; }

		/// <summary>
		/// Отменено пользователем.
		/// Сообщение для исключения при отмене команды пользователем.
		/// </summary>
		public const string CanceledByUser = "Отменено пользователем";        

        /// <summary>
        /// Символы строковые
        /// </summary>
        public static class Symbols
        {
            /// <summary>
            /// Диаметр ⌀
            /// </summary>
            public const string Diam = "⌀";
            /// <summary>
            /// Кубическая степень- ³
            /// </summary>
            public const string Cubic = "³";
            /// <summary>
            /// Квадратная степень- ²
            /// </summary>
            public const string Square = "²";
            /// <summary>
            /// Градус - °
            /// </summary>
            public const string Degree = "°";
        }

        /// <summary>
        /// Файл из папки пользовательских данных (AppData\PIK\Autocad\...)
        /// </summary>
        /// <param name="pluginName">Имя программы(плагина)</param>
        /// <param name="fileName">Имя файла</param>
        [Obsolete("Используй Path.GetUserPluginFolder()")]
        public static string GetUserDataFile (string pluginName, string fileName)
        {
            return IO.Path.GetUserPluginFile(pluginName, fileName);   
        }        

        public static bool IsCadManager()
        {
            return Environment.UserName.Equals(PikSettings.PikFileSettings.LoginCADManager, StringComparison.OrdinalIgnoreCase);
        }
    }
}
