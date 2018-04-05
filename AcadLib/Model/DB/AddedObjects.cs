using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;

namespace AcadLib
{
    /// <summary>
    /// Собирает добавленные в базу чертежа объекты
    /// </summary>
    [PublicAPI]
    public class AddedObjects : IDisposable
    {
        private readonly Database db;

        public List<ObjectId> Added { get;  } = new List<ObjectId>();
        public event ObjectEventHandler ObjectAppended;

        public AddedObjects([NotNull] Database db)
        {
            this.db = db;
            db.ObjectAppended += Db_ObjectAppended;
        }

        private void Db_ObjectAppended(object sender, [NotNull] ObjectEventArgs e)
        {
            Added.Add(e.DBObject.Id);
            ObjectAppended?.Invoke(sender, e);
        }

        public void Dispose()
        {
            db.ObjectAppended -= Db_ObjectAppended;
        }
    }
}
