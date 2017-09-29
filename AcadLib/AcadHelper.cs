using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace AcadLib
{
    public static class AcadHelper
    {
        /// <summary>
        /// Текущий документ
        /// </summary>
        public static Document Doc => Application.DocumentManager.MdiActiveDocument;
    }
}
