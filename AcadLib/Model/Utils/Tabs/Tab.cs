namespace AcadLib.Utils.Tabs
{
    using Autodesk.AutoCAD.ApplicationServices;

    internal class Tab
    {
        public Document Doc { get; }

        public Tab(Document doc)
        {
            Doc = doc;
        }
    }
}