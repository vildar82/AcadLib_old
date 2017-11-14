using System;
using Autodesk.AutoCAD.ApplicationServices;

namespace AcadLib
{
    public static class AcadHelper
    {
        public static int VersionMajor => Application.Version.Major;
        /// <summary>
        /// Текущий документ
        /// </summary>
        public static Document Doc => Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager?.MdiActiveDocument;

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
