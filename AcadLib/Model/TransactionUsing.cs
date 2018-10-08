namespace AcadLib
{
    using System;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using JetBrains.Annotations;

    public class TransactionUsing : IDisposable
    {
        public TransactionUsing()
        {
            Doc = AcadHelper.Doc;
            T = Doc.TransactionManager.StartTransaction();
        }

        public TransactionUsing([NotNull] Document doc)
        {
            Doc = doc;
            T = doc.TransactionManager.StartTransaction();
        }

        /// <summary>
        /// Запущенная транзакция
        /// </summary>
        [NotNull]
        public Transaction T { get; }

        /// <summary>
        /// Текущий документ
        /// </summary>
        [NotNull]
        public Document Doc { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            T?.Commit();
        }
    }
}