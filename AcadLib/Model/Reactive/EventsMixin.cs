namespace AcadLib.Reactive
{
    using Autodesk.AutoCAD.DatabaseServices;

    public static class EventsMixin
    {
        public static DboEvents Events(this DBObject dbo)
        {
            return new DboEvents(dbo);
        }
    }
}