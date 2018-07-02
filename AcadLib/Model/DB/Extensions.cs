// ReSharper disable once CheckNamespace
namespace Autodesk.AutoCAD.DatabaseServices
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AcadLib;
    using JetBrains.Annotations;

    [PublicAPI]
    public static class DatabaseExtensions
    {
        // Opens a DBObject in ForRead mode (kaefer @ TheSwamp)
        [CanBeNull]
        public static T GetObject<T>(this ObjectId id) where T : DBObject
        {
            return id.GetObject<T>(OpenMode.ForRead);
        }

        [CanBeNull]
        public static T GetObject<T>(this ObjectId id, [NotNull] Transaction t) where T : DBObject
        {
            return id.GetObject<T>(OpenMode.ForRead, t);
        }

        [NotNull]
        public static T GetObjectT<T>(this ObjectId id) where T : DBObject
        {
            return id.GetObjectT<T>(OpenMode.ForRead);
        }

        [NotNull]
        public static T GetObjectT<T>(this ObjectId id, [NotNull] Transaction t) where T : DBObject
        {
            return id.GetObjectT<T>(OpenMode.ForRead, t);
        }

        // Opens a DBObject in the given mode (kaefer @ TheSwamp)
        [CanBeNull]
        public static T GetObject<T>(this ObjectId id, OpenMode mode) where T : DBObject
        {
            if (!id.IsValidEx())
                return null;
            return id.GetObject(mode, false, true) as T;
        }

        [CanBeNull]
        public static T GetObject<T>(this ObjectId id, OpenMode mode, [NotNull] Transaction t) where T : DBObject
        {
            if (!id.IsValidEx())
                return null;
            return t.GetObject(id, mode, false, true) as T;
        }

        [NotNull]
        public static T GetObjectT<T>(this ObjectId id, OpenMode mode) where T : DBObject
        {
            if (!id.IsValidEx())
                throw new InvalidOperationException();
            return (T)id.GetObject(mode, false, true);
        }

        [NotNull]
        public static T GetObjectT<T>(this ObjectId id, OpenMode mode, [NotNull] Transaction t) where T : DBObject
        {
            if (!id.IsValidEx())
                throw new InvalidOperationException();
            return (T)t.GetObject(id, mode, false, true);
        }

        // Opens a collection of DBObject in ForRead mode (kaefer @ TheSwamp)
        [NotNull]
        public static IEnumerable<T> GetObjects<T>([NotNull] this IEnumerable ids) where T : DBObject
        {
            return ids.GetObjects<T>(OpenMode.ForRead);
        }

        [NotNull]
        public static IEnumerable<T> GetObjects<T>([NotNull] this IEnumerable ids, Transaction t) where T : DBObject
        {
            return ids.GetObjects<T>(OpenMode.ForRead, t);
        }

        // Opens a collection of DBObject in the given mode (kaefer @ TheSwamp)
        [NotNull]
        public static IEnumerable<T> GetObjects<T>([NotNull] this IEnumerable ids, OpenMode mode) where T : DBObject
        {
            return ids
                .Cast<ObjectId>()
                .Select(id => id.GetObject<T>(mode))
                .Where(res => res != null);
        }

        [NotNull]
        public static IEnumerable<T> GetObjects<T>([NotNull] this IEnumerable ids, OpenMode mode, Transaction t)
            where T : DBObject
        {
            return ids
                .Cast<ObjectId>()
                .Select(id => id.GetObject<T>(mode, t))
                .Where(res => res != null);
        }

        /// <summary>
        /// Имя блока в том числе динамического.
        /// Без условия открытой транзакции.
        /// br.DynamicBlockTableRecord.Open(OpenMode.ForRead)
        /// </summary>
        [Obsolete("Use DictBlockNames")]
        public static string GetEffectiveName([NotNull] this BlockReference br)
        {
#pragma warning disable 618
            using (var btrDyn = (BlockTableRecord)br.DynamicBlockTableRecord.Open(OpenMode.ForRead))
#pragma warning restore 618
            {
                return btrDyn.Name;
            }
        }
    }
}