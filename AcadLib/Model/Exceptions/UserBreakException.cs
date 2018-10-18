namespace AcadLib
{
    using System;

    /// <summary>
    /// Прерывание долгого процесса пользователем.
    /// Используется в случае екстремального прерывания - выбрасывается исключение из палитровых методов.
    /// </summary>
    [Obsolete("Use OperationCanceledException")]
    public class UserBreakException : Exception
    {
    }
}