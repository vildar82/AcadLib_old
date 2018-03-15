using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.Runtime;
using static Autodesk.AutoCAD.ApplicationServices.Core.Application;

using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib
{
    [PublicAPI]
    public static class AcadHelper
    {
        /// <summary>
        /// Это русская версия AutoCAD ru-RU
        /// </summary>
        /// <returns></returns>
        public static bool IsRussianAcad()
        {
            return SystemObjects.DynamicLinker.ProductLcid == 419;
        }

        /// <summary>
        /// Текущий документ.
        /// </summary>
        /// <exception cref="InvalidOperationException">Если нет активного чертежа.</exception>
        [NotNull]
        public static Document Doc => DocumentManager.MdiActiveDocument ?? throw new InvalidOperationException();
        public static int VersionMajor => Application.Version.Major;

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
    }
}