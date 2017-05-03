using Autodesk.AutoCAD.ApplicationServices;

namespace AcadLib.XData
{
    public interface IExtDataSave
    {
        /// <summary>
        /// Словарь для сохранения объекта
        /// </summary>        
        DicED GetExtDic (Document doc);
        /// <summary>
        /// установить значения из словаря в объект
        /// </summary>        
        void SetExtDic (DicED dicEd, Document doc);
    }
}
