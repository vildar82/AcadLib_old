using System;

// ReSharper disable once CheckNamespace
namespace AcadLib
{
    /// <summary>
    /// Прерывание долгого процесса пользователем.
    /// Используется в случае екстремального прерывания - выбрасывается исключение из палитровых методов.
    /// </summary>
    [Obsolete("Use OperationCanceledException")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserBreakException : Exception
    {
    }
}