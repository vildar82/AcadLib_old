namespace AcadLib.UI.Ribbon.Elements
{
    using System.Collections.Generic;
    using NetLib.WPF;

    public abstract class RibbonItemData : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsTest { get; set; }

        /// <summary>
        /// Доступ - имена групп и логины.
        /// </summary>
        public List<string> Access { get; set; }
    }
}
