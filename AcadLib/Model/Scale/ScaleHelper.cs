using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Scale
{
    public static class ScaleHelper
    {
        /// <summary>
        /// Текущий масштаб аннотаций.
        /// </summary>        
        public static double GetCurrentAnnoScale(Database db)
        {
            return 1 / db.Cannoscale.Scale;
        }
    }
}
