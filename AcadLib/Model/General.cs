using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCAD_PIK_Manager.Settings;

namespace AcadLib
{
    public static class General
    {
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
