using System;

namespace AcadLib
{
    /// <summary>
    /// Прерывание долгого процесса пользователем.
    /// Используется в случае екстремального прерывания - выбрасывается исключение из палитровых методов.
    /// </summary>
    public class UserBreakException : Exception
    {
    }
}
