namespace AcadLib
{
    using System;

    [Obsolete("Use OperationCanceledException")]
    public class CancelByUserException : Exception
    {
        public override string Message { get; } = General.CanceledByUser;
    }
}