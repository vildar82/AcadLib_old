namespace AcadLib.Styles.StyleManager.UI
{
    using Autodesk.AutoCAD.DatabaseServices;
    using NetLib.WPF;

    public class Style : BaseModel
    {
        public Style(StyleManagerVM baseVM) : base(baseVM)
        {
        }

        public string Name { get; set; }

        public ObjectId Id { get; set; }
    }
}