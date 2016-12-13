using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.Blocks
{
    public enum PropertyType
    {
        /// <summary>
        /// Не установлено
        /// </summary>
        None,
        /// <summary>
        /// Аттрибут
        /// </summary>
        Attribute,
        /// <summary>
        /// Динамическое свойство
        /// </summary>
        Dynamic
    }
}
