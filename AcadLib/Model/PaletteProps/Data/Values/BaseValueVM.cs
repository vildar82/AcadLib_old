namespace AcadLib.PaletteProps
{
    using System;
    using NetLib.WPF;

    public abstract class BaseValueVM : BaseModel
    {
        public bool IsReadOnly { get; set; }

        protected static void Update<T>(T value, Action<T> update)
        {
            var doc = AcadHelper.Doc;
            using (doc.LockDocument())
            using (var t = doc.TransactionManager.StartTransaction())
            {
                update(value);
                t.Commit();
            }
        }
    }
}