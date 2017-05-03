using System;
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
            try
            {
                return 1 / db.Cannoscale.Scale;
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex, "GetCurrentAnnoScale");
            }
            return 1;
        }
    }
}
