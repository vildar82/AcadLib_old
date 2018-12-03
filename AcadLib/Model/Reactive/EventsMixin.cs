using Autodesk.AutoCAD.ApplicationServices;
using JetBrains.Annotations;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

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
        
        [NotNull]
        public static DocumentsEvents Events([NotNull] this DocumentCollection docMan)
        {
            return new DocumentsEvents(docMan);
        }
    }
}