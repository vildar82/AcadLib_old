using System;
using Autodesk.AutoCAD.ApplicationServices;
using JetBrains.Annotations;
using static Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib
{
    public static class AcadHelper
    {
        public static int VersionMajor => Application.Version.Major;
        /// <summary>
        /// Текущий документ.
        /// </summary>
        /// <exception cref="InvalidOperationException">Если нет активного чертежа.</exception>
        [NotNull]
        public static Document Doc => DocumentManager.MdiActiveDocument ?? throw new InvalidOperationException();

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
