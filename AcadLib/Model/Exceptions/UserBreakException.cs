using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
