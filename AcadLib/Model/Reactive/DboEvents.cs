using System.Reactive;

namespace AcadLib.Reactive
{
    using System;
    using System.Reactive.Linq;
    using Autodesk.AutoCAD.DatabaseServices;

    public class DboEvents
    {
        private readonly DBObject _dbo;

        public DboEvents(DBObject dbo)
        {
            _dbo = dbo;
        }

        public IObservable<EventPattern<EventArgs>> Modified => Observable.FromEventPattern<EventHandler, EventArgs>(
            x => _dbo.Modified += x, x => _dbo.Modified -= x);

        public IObservable<EventPattern<ObjectEventArgs>> Copied => Observable.FromEventPattern<ObjectEventHandler, ObjectEventArgs>(
            x => _dbo.Copied += x, x => _dbo.Copied -= x);

        public IObservable<EventPattern<ObjectClosedEventArgs>> ObjectClosed => Observable
            .FromEventPattern<ObjectClosedEventHandler, ObjectClosedEventArgs>(
                x => _dbo.ObjectClosed += x, x => _dbo.ObjectClosed -= x);
    }
}