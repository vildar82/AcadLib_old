namespace AcadLib.DB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.AutoCAD.DatabaseServices;

    /// <summary>
    /// Копирование объектов из внешней базы
    /// </summary>
    public static class ExternalCopyObjectsExt
    {
        /// <summary>
        /// Копирование объектов из внешней базы
        /// </summary>
        /// <param name="dbDest">База целевая</param>
        /// <param name="externalFile">Внешний файл</param>
        /// <param name="getOwnerId">Получение таблицы содержащей копируемые элементы</param>
        /// <param name="getCopyIds">Получение списка копируемых объектов из таблицы</param>
        /// <param name="mode">Режим копирования</param>
        public static List<ObjectId> Copy(this Database dbDest, string externalFile, DuplicateRecordCloning mode,
            Func<Database, ObjectId> getOwnerId, Func<ObjectId, List<ObjectId>> getCopyIds)
        {
            using (var dbSource = new Database(false, true))
            {
                dbSource.ReadDwgFile(externalFile, FileOpenMode.OpenForReadAndAllShare, false, string.Empty);
                dbSource.CloseInput(true);

                List<ObjectId> idsSource;
                ObjectId ownerIdDest;
                using (var t = dbSource.TransactionManager.StartTransaction())
                {
                    var ownerIdSourse = getOwnerId(dbSource);
                    ownerIdDest = getOwnerId(dbDest);
                    idsSource = getCopyIds(ownerIdSourse);
                    t.Commit();
                }

                if (idsSource?.Any() != true)
                    return new List<ObjectId>();

                using (var map = new IdMapping())
                using (var ids = new ObjectIdCollection(idsSource.ToArray()))
                {
                    dbDest.WblockCloneObjects(ids, ownerIdDest, map, mode, false);
                    return idsSource.Select(s => map[s].Value).ToList();
                }
            }
        }
    }
}
