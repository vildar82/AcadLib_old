using System;

namespace AcadLib
{
    public class CancelByUserException : Exception
    {
        public override string Message { get; } = General.CanceledByUser;        
    }
}
