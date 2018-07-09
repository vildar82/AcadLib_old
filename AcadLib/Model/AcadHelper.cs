
namespace AcadLib
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.Runtime;
    using JetBrains.Annotations;
    using static Autodesk.AutoCAD.ApplicationServices.Core.Application;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
    using Exception = Autodesk.AutoCAD.Runtime.Exception;

    /// <summary>
    /// Вспомогательные функции для работы с автокадом
    /// </summary>
    [PublicAPI]
    public static class AcadHelper
    {
        /// <summary>
        /// Текущий документ.
        /// </summary>
        /// <exception cref="InvalidOperationException">Если нет активного чертежа.</exception>
        [NotNull]
        public static Document Doc => DocumentManager.MdiActiveDocument ?? throw new InvalidOperationException();

        /// <summary>
        /// Основной номер версии Автокада
        /// </summary>
        public static int VersionMajor => Application.Version.Major;

        /// <summary>
        /// Это русская версия AutoCAD ru-RU
        /// </summary>
        /// <returns></returns>
        public static bool IsRussianAcad()
        {
            return SystemObjects.DynamicLinker.ProductLcid == 1049;
        }

        [CanBeNull]
        public static Document GetOpenedDocument(string file)
        {
            return DocumentManager.Cast<Document>().FirstOrDefault(d =>
                Path.GetFullPath(d.Name).Equals(Path.GetFullPath(file), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Если пользователь нажал Esc для прерывания процесса
        /// </summary>
        /// <returns></returns>
        public static bool UserBreak()
        {
            return HostApplicationServices.Current.UserBreak();
        }

        /// <summary>
        /// Сообщение в ком.строку. автокада
        /// </summary>
        public static void WriteLine(string msg)
        {
            try
            {
                Doc.Editor.WriteMessage($"\n{msg}");
            }
            catch
            {
                //
            }
        }

        public static void WriteToCommandLine(this string msg)
        {
            try
            {
                Doc.Editor.WriteMessage($"\n{msg}");
            }
            catch
            {
                //
            }
        }

        /// <summary>
        /// Определение, что только один автокад запущен
        /// </summary>
        public static bool IsOneAcadRun()
        {
            try
            {
                return !Process.GetProcessesByName("acad").Where(IsValidAcadProcess).Skip(1).Any();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "IsOneAcadRun");
                return true;
            }
        }

        private static bool IsValidAcadProcess(Process process)
        {
            try
            {
                // На "липовом" процессе acad.exe - выскакивает исключение. Обнаружисоль в Новороссийске у Жуковой Юли/
                var unused = process.VirtualMemorySize64;
                if (process.NonpagedSystemMemorySize64 < 20000)
                    return false;
                var unused1 = process.MainWindowTitle;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}