using AcadLib;
using Autodesk.AutoCAD.Geometry;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Autodesk.AutoCAD.DatabaseServices
{
    [PublicAPI]
    public static class BlockExtents
    {
        private static readonly Scale3d scale1 = new Scale3d(1);
        private static readonly Tolerance tolerance = new Tolerance(0.001, 0.001);

        public static bool IsScaleEquals1([NotNull] this BlockReference blref)
        {
            return blref.ScaleFactors.IsEqualTo(scale1, tolerance);
        }

        public static bool IsScaleEquals([NotNull] this BlockReference blref, int scale)
        {
            return blref.ScaleFactors.IsEqualTo(new Scale3d(scale), tolerance);
        }

        /// <summary>
        /// Обновление графики во вхождениях блока для данного определения блока
        /// Должна быть запущена транзакция!!!
        /// </summary>
        public static void SetBlRefsRecordGraphicsModified([NotNull] this BlockTableRecord btr)
        {
            var idsBlRef = btr.GetBlockReferenceIds(true, false);
            foreach (ObjectId idBlRefApart in idsBlRef)
            {
                var blRefApartItem = (BlockReference)idBlRefApart.GetObject(OpenMode.ForWrite, false, true);
                blRefApartItem.RecordGraphicsModified(true);
            }
        }

        /// <summary>
        /// Определение границы блока чистых (без динамики, без атрибутов)
        /// </summary>
        /// <param name="blRef"></param>
        /// <returns></returns>
        public static Extents3d GeometricExtentsСlean(this BlockReference blRef)
        {
            var mat = Matrix3d.Identity;
            var blockExt = GetBlockExtents(blRef, ref mat, new Extents3d());
            return blockExt;
        }

        /// <summary>
        /// Рекурсивное получение габаритного контейнера для выбранного примитива.
        /// </summary>
        /// <param name="en">Имя примитива</param>
        /// <param name="ext">Габаритный контейнер</param>
        /// <param name="mat">Матрица преобразования из системы координат блока в МСК.</param>
        private static Extents3d GetBlockExtents(Entity en, ref Matrix3d mat, Extents3d ext)
        {
            if (en is BlockReference bref)
            {
                var matIns = mat * bref.BlockTransform;
#pragma warning disable 618
                using (var btr = (BlockTableRecord)bref.BlockTableRecord.Open(OpenMode.ForRead))
#pragma warning restore 618
                {
                    foreach (var id in btr)
                    {
                        // Пропускаем все тексты.
                        if (id.ObjectClass.IsDerivedFrom(General.ClassDbTextRX) ||
                           id.ObjectClass.IsDerivedFrom(General.ClassMTextRX) ||
                           id.ObjectClass.IsDerivedFrom(General.ClassMLeaderRX) ||
                           id.ObjectClass.IsDerivedFrom(General.ClassDimension))
                        {
                            continue;
                        }
#pragma warning disable 618
                        using (var obj = id.Open(OpenMode.ForRead, false, true))
#pragma warning restore 618
                        {
                            var enCur = obj as Entity;
                            if (enCur == null || enCur.Visible != true)
                                continue;
                            if (IsEmptyExt(ref ext))
                            {
                                ext.AddExtents(GetBlockExtents(enCur, ref matIns, ext));
                            }
                            else
                            {
                                ext = GetBlockExtents(enCur, ref matIns, ext);
                            }
                        }
                    }
                }
            }
            else
            {
                if (mat.IsUniscaledOrtho())
                {
                    using (var enTr = en.GetTransformedCopy(mat))
                    {
                        (enTr as Dimension)?.RecomputeDimensionBlock(true);
                        (enTr as Table)?.RecomputeTableBlock(true);

                        if (IsEmptyExt(ref ext))
                        {
                            try { ext = enTr.GeometricExtents; }
                            catch
                            {
                                // ignored
                            }
                        }
                        else
                        {
                            try { ext.AddExtents(enTr.GeometricExtents); }
                            catch
                            {
                                // ignored
                            }
                        }
                        return ext;
                    }
                }
                try
                {
                    var curExt = en.GeometricExtents;
                    curExt.TransformBy(mat);
                    if (IsEmptyExt(ref ext))
                        ext = curExt;
                    else
                        ext.AddExtents(curExt);
                }
                catch
                {
                    // ignored
                }
                return ext;
            }
            return ext;
        }

        /// <summary>
        /// Определят не пустой ли габаритный контейнер.
        /// </summary>
        /// <param name="ext">Габаритный контейнер.</param>
        /// <returns></returns>
        public static bool IsEmptyExt(ref Extents3d ext)
        {
            return ext.MinPoint.DistanceTo(ext.MaxPoint) < Tolerance.Global.EqualPoint;
        }
    }
}