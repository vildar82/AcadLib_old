using System;
using NetLib.WPF;

namespace AcadLib.PaletteProps
{
    public abstract class BaseValueVM : BaseModel
    {
        public bool IsReadOnly { get; set; }

        protected static void Update<T>(T color, Action<T> update)
        {
            var doc = AcadHelper.Doc;
            using (doc.LockDocument())
            using (var t = doc.TransactionManager.StartTransaction())
            {
                update(color);
                t.Commit();
            }
        }
    }
}
