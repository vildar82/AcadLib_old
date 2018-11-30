using System;
using System.Reactive;
using System.Reactive.Linq;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Reactive
{
    public class DbEvents
    {
        private readonly Database db;

        public DbEvents(Database db)
        {
            this.db = db;
        }

        public IObservable<EventPattern<DatabaseIOEventArgs>> SaveComplete =>
            Observable.FromEventPattern<DatabaseIOEventHandler, DatabaseIOEventArgs>
                (x => db.SaveComplete += x, x => db.SaveComplete -= x);
    }
}