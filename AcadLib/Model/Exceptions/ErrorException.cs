using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Exceptions
{
    public class ErrorException : Exception
    {
        public Errors.Error Error { get; private set; }

        public ErrorException(Errors.Error err):base(err.Message)
        {
            Error = err;
        }

        public ErrorException (string msg, Entity ent, Icon icon) : base(msg)
        {
            Error = new Errors.Error(msg, ent, icon);
        }

        public ErrorException(string msg, Icon icon) : base(msg)
        {
            Error = new Errors.Error(msg, icon);
        }       
    }
}
