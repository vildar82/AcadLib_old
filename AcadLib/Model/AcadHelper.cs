using Autodesk.AutoCAD.ApplicationServices;
using JetBrains.Annotations;
using static Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib
{
    public static class AcadHelper
    {
        public static int VersionMajor => Version.Major;
        /// <summary>
        /// Текущий документ
        /// </summary>
        [NotNull]
        public static Document Doc => DocumentManager.MdiActiveDocument;

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
