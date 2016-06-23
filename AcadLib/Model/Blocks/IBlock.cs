using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Blocks
{
    /// <summary>
    /// Блок на чертеже
    /// </summary>
    public interface IBlock
    {
        /// <summary>
        /// Эффективное имя блока
        /// </summary>
        string BlName { get; set; }
        ObjectId IdBlRef { get; set; }
        ObjectId IdBtr { get; set; }
        Extents3d? Bounds { get; set; }
        /// <summary>
        /// Параметры (атр + дин)
        /// </summary>
        List<Property> Properties { get; set; }
        /// <summary>
        /// Границы для показа
        /// </summary>
        Extents3d ExtentsToShow { get; set; }
        /// <summary>
        /// Показать блок на чертеже
        /// </summary>
        void Show ();        
    }
}
