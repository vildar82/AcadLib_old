using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.Files
{
    public static class CadSettingsPath
    {
        /// <summary>
        /// Определение альтернативного пути к диску Z
        /// </summary>        
        public static string GetCadSettingsRealPath(this string path)
        {
            // Если строка начинается с диска Z, а его не существует, то подбор альтернативного пути к диску Z
            if (path.StartsWith("z", StringComparison.OrdinalIgnoreCase) && !path.PathExists())
            {
                var res = Path.Combine(@"\\dsk2.picompany.ru\project\CAD_Settings", path.Substring(3));
                if (!res.PathExists())
                {
                    res = Path.Combine(@"\\ab4\CAD_Settings", path.Substring(3));
                    if (!res.PathExists())
                    {
                        Logger.Log.Error($"Сетевой путь к настройкам недоступен - {path}");
                        throw new FileNotFoundException($"Не определен путь {path}");
                    }
                }
                return res;
            }                          
            return path;
        }        

        public static bool PathExists(this string path)
        {
            return (Directory.Exists(path) || File.Exists(path));
        }        
    }
}
