using System;

namespace AcadLib
{
    [Obsolete("Use OperationCanceledException")]
    public class CancelByUserException : Exception
    {
        public override string Message { get; } = General.CanceledByUser;        
    }
}
