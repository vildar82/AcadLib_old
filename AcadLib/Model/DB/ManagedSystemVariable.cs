using JetBrains.Annotations;
using System;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib
{
    /// <summary>
    /// Automates saving/changing/restoring system variables
    /// </summary>
    public class ManagedSystemVariable : IDisposable
    {
        string name = null;
        object oldval = null;

        public ManagedSystemVariable(string name, object value)
           : this(name)
        {
            Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable(name, value);
        }

        public ManagedSystemVariable([NotNull] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(name);
            this.name = name;
            oldval = Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable(name);
        }

        public void Dispose()
        {
            if (oldval != null)
            {
                var temp = oldval;
                oldval = null;
                Application.SetSystemVariable(name, temp);
            }
        }
    }
}
