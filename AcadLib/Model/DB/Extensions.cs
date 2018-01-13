using System;
using AcadLib;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Autodesk.AutoCAD.DatabaseServices
{
    public static class DatabaseExtensions
    {
        // Opens a DBObject in ForRead mode (kaefer @ TheSwamp)
        [CanBeNull]
        public static T GetObject<T>(this ObjectId id) where T : DBObject
        {
            return id.GetObject<T>(OpenMode.ForRead);
        }

        // Opens a DBObject in the given mode (kaefer @ TheSwamp)
        [CanBeNull]
        public static T GetObject<T>(this ObjectId id, OpenMode mode) where T : DBObject
        {
            if (!id.IsValidEx())
                return null;
            return id.GetObject(mode, false, true) as T;
        }

        // Opens a collection of DBObject in ForRead mode (kaefer @ TheSwamp)
        [NotNull]
        public static IEnumerable<T> GetObjects<T>([NotNull] this IEnumerable ids) where T : DBObject
        {
            return ids.GetObjects<T>(OpenMode.ForRead);
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