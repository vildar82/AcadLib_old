using System;

// ReSharper disable once CheckNamespace
namespace AcadLib
{
    [Obsolete("Use OperationCanceledException")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CancelByUserException : Exception
    {
        public override string Message { get; } = General.CanceledByUser;
    }
}