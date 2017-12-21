using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib
{
    public static class SystemVariableExt
    {
        public static void SetSystemVariable(this string name, object value)
        {
            Application.SetSystemVariable(name, value);
        }

        public static void SetSystemVariableTry(this string name, object value)
        {
            try
            {
                Application.SetSystemVariable(name, value);
            }
            catch(Exception ex)
            {
                Logger.Log.Error($"SetSystemVariableTry name={name}, value={value}", ex);
            }
        }

        public static object GetSystemVariable(this string name)
        {
            return Application.GetSystemVariable(name);
        }
    }
}
