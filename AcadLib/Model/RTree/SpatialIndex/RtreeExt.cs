namespace AcadLib.RTree.SpatialIndex
{
    using System;
    using System.Collections.Generic;
    using Autodesk.AutoCAD.DatabaseServices;
    using JetBrains.Annotations;

    [PublicAPI]
    public static class RtreeExt
    {
        [NotNull]
        public static RTree<T> ToRTree<T>(this IEnumerable<T> ents)
            where T : Entity
        {
            var tree = new RTree<T>();
            foreach (var ent in ents)
            {
                tree.Add(new Rectangle(ent.GeometricExtents), ent);
            }

            return tree;
        }

        [NotNull]
        public static RTree<T> ToRTree<T>(this IEnumerable<T> ents, Func<T, Extents3d> getExt)
        {
            var tree = new RTree<T>();
            foreach (var ent in ents)
            {
                tree.Add(new Rectangle(getExt(ent)), ent);
            }

            return tree;
        }

        [NotNull]
        public static RTree<T> ToRTree2d<T>(this IEnumerable<T> ents)
            where T : Entity
        {
            var tree = new RTree<T>();
            foreach (var ent in ents)
            {
                tree.Add(new Rectangle(ent.GeometricExtents.Convert2d()), ent);
            }

            return tree;
        }

        [NotNull]
        public static RTree<T> ToRTree2d<T>(this IEnumerable<T> ents, Func<T, Extents3d> getExt)
        {
            var tree = new RTree<T>();
            foreach (var ent in ents)
            {
                tree.Add(new Rectangle(getExt(ent).Convert2d()), ent);
            }

            return tree;
        }
    }
}
