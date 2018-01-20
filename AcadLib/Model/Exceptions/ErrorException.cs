using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System;
using System.Drawing;

namespace AcadLib.Exceptions
{
    /// <summary>
    /// Ошибка с Error (можно показать объект на чертеже)
    /// </summary>
    [PublicAPI]
    public class ErrorException : Exception
    {
        public Errors.Error Error { get; private set; }

        public ErrorException([NotNull] Errors.Error err) : base(err.Message)
        {
            Error = err;
        }

        public ErrorException(string msg, [NotNull] Entity ent, Icon icon) : base(msg)
        {
            Error = new Errors.Error(msg, ent, icon);
        }

        public ErrorException(string msg, Icon icon) : base(msg)
        {
            Error = new Errors.Error(msg, icon);
        }
    }
}