using UnitsNet;

namespace AcadLib.Units
{
    /// <summary>
    /// Конвертер единиц
    /// </summary>
    public static class UnitsConvertHelper
    {
        /// <summary>
        /// Преобразование длины из мм в метры.
        /// </summary>        
        public static double ConvertMmToMLength (double mm)
        {
            var len = Length.FromMillimeters(mm);
            return len.Meters;
        }
    }
}
