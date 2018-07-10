namespace AcadLib.RTree.SpatialIndex
{
    using System.Collections.Generic;
    using Autodesk.AutoCAD.DatabaseServices;
    using JetBrains.Annotations;

    [PublicAPI]
    public static class RtreeExt
    {
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
    }
}
