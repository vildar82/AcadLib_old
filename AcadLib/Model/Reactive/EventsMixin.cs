using JetBrains.Annotations;

namespace AcadLib.Reactive
{
    using Autodesk.AutoCAD.DatabaseServices;

    public static class EventsMixin
    {
        [NotNull]
        public static DboEvents Events([NotNull] this DBObject dbo)
        {
            return new DboEvents(dbo);
        }
        
        [NotNull]
        public static DbEvents Events([NotNull] this Database db)
        {
            return new DbEvents(db);
        }
    }
}