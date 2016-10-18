using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Blocks
{
    /// <summary>
    /// Блок на чертеже
    /// </summary>
    public interface IBlock : IEquatable<IBlock>
    {
        Database Db { get; }
        /// <summary>
        /// Эффективное имя блока
        /// </summary>
        string BlName { get; set; }
        /// <summary>
        /// Слой на котором расположен блок
        /// </summary>
        string BlLayer { get; set; }
        Point3d Position { get;}
        Color Color { get;}
        ObjectId IdBlRef { get; set; }
        ObjectId IdBtr { get; set; }
        /// <summary>
        /// Пространство в который вставлен этот блок (определение блока)
        /// </summary>
        ObjectId IdBtrOwner { get; }
        Extents3d? Bounds { get; set; }
        /// <summary>
        /// Параметры (атр + дин)
        /// </summary>
        List<Property> Properties { get; set; }
        /// <summary>
        /// Границы для показа
        /// </summary>
        Extents3d ExtentsToShow { get; }
        Matrix3d Transform { get; }
        Error Error { get; set; }
        /// <summary>
        /// Показать блок на чертеже
        /// </summary>
        void Show ();
        /// <summary>
        /// Получение свойства блока
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="propMatch">Соответствие имени свойства</param>
        /// <param name="isRequired">Обязательное свойство. Будет добалена ошибка в Error.</param>
        /// <param name="exactMatch">точное соответствие имени свойства</param>        
        T GetPropValue<T> (string propMatch, bool isRequired = true, bool exactMatch = true);
        /// <summary>
        /// Запись свойства
        /// </summary>
        /// <param name="propMatch">Соответствие имени свойства</param>
        /// <param name="value">Значение</param>
        /// <param name="exactMatch">Точное соответствие имени свойства</param>
        /// <param name="isRequired">Обязательное свойство? Если да, а свойства не найдено, будет добавлена ошибка</param>
        void FillPropValue (string propMatch, object value, bool exactMatch = true, bool isRequired = true);
        void AddError (string err);
        void Update (BlockReference blRef);
    }
}
