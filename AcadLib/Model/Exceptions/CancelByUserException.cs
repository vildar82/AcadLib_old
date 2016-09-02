using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib
{
    public class CancelByUserException : Exception
    {
        public override string Message { get; } = General.CanceledByUser;        
    }
}
