using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace AcadLib.Visual
{
    /// <summary>
    /// Элемент который может визуализироваться
    /// </summary>
    public interface IVisualElement
    {
        /// <summary>
        /// Создание элементов визуализации
        /// </summary>        
        List<Entity> CreateVisual();
    }
}
