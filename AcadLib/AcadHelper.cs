using Autodesk.AutoCAD.ApplicationServices;

namespace AcadLib
{
    public static class AcadHelper
    {
        public static int VersionMajor => Application.Version.Major;
        /// <summary>
        /// Текущий документ
        /// </summary>
        public static Document Doc => Application.DocumentManager.MdiActiveDocument;

        /// <summary>
        /// Сообщение в ком.строку. автокада
        /// </summary>
        public static void WriteLine(string msg)
        {
            Doc.Editor.WriteMessage($"\n{msg}");
        }
        public static void WriteToCommandLine(this string msg)
        {
            Doc.Editor.WriteMessage($"\n{msg}");
        }
    }
}
