using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib
{
    public static class Logger
    {
        public static LoggAddinExt Log;

        static Logger ()
        {
            Log = new LoggAddinExt(AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup);
        }        
    }

    public class LoggAddinExt : AutoCAD_PIK_Manager.LogAddin
    {
        public LoggAddinExt (string plugin) : base(plugin)
        {
        }

        /// <summary>
        /// Отзыв
        /// </summary>        
        public void Report (string msg)
        {
            Error("#Report: " + msg);
        }
    }

}
