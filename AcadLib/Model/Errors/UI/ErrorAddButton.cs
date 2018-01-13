using System.Windows.Input;

namespace AcadLib.Errors.UI
{
    public class ErrorAddButton
    {
        /// <summary>
        /// Имя на кнопке
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание кнопки
        /// </summary>
        public string Tooltip { get; set; }
        /// <summary>
        /// Обработчик нажатия на кнопку
        /// </summary>
        public ICommand Click { get; set; }
    }
}
