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

        public IObservable<EventArgs> Modified => Observable.FromEventPattern(
            x => _dbo.Modified += x, x => _dbo.Modified -= x).Select(x => x.EventArgs);

        public IObservable<ObjectEventArgs> Copied => Observable.FromEventPattern<ObjectEventHandler, ObjectEventArgs>(
            x => _dbo.Copied += x, x => _dbo.Copied -= x).Select(x => x.EventArgs);

        public IObservable<ObjectClosedEventArgs> ObjectClosed => Observable
            .FromEventPattern<ObjectClosedEventHandler, ObjectClosedEventArgs>(
                x => _dbo.ObjectClosed += x, x => _dbo.ObjectClosed -= x).Select(x => x.EventArgs);
    }
}