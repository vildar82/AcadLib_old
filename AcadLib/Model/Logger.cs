using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib
{
    public static class Logger
    {
        public static AutoCAD_PIK_Manager.LogAddin Log;

        static Logger ()
        {
            Log = new AutoCAD_PIK_Manager.LogAddin(AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup);
        }        
    }
}
