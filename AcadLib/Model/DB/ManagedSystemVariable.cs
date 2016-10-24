using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace AcadLib
{
    /// <summary>
    /// Automates saving/changing/restoring system variables
    /// </summary>

    public class ManagedSystemVariable : IDisposable
    {
        string name = null;
        object oldval = null;

        public ManagedSystemVariable (string name, object value)
           : this(name)
        {
            Application.SetSystemVariable(name, value);
        }

        public ManagedSystemVariable (string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(name);
            this.name = name;
            this.oldval = Application.GetSystemVariable(name);
        }

        public void Dispose ()
        {
            if (oldval != null)
            {
                object temp = oldval;
                oldval = null;
                Application.SetSystemVariable(name, temp);
            }
        }
    }
}
