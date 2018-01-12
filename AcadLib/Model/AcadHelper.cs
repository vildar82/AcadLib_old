using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System;
using static Autodesk.AutoCAD.ApplicationServices.Core.Application;

using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib
{
    [PublicAPI]
    public static class AcadHelper
    {
        /// <summary>
        /// Текущий документ.
        /// </summary>
        /// <exception cref="InvalidOperationException">Если нет активного чертежа.</exception>
        [NotNull]
        public static Document Doc => DocumentManager.MdiActiveDocument ?? throw new InvalidOperationException();
        public static int VersionMajor => Application.Version.Major;

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