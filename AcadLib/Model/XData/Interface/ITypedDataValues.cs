using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.XData
{
    public interface ITypedDataValues
    {
        /// <summary>
        /// Список сохраняемяхъ значений
        /// </summary>
        /// <returns></returns>
        List<TypedValue> GetDataValues (Document doc);
        /// <summary>
        /// Установка значений
        /// </summary>
        /// <param name="values"></param>
        void SetDataValues (List<TypedValue> values, Document doc);
    }
}
