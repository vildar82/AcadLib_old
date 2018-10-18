namespace AcadLib.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.Geometry;
    using JetBrains.Annotations;
    using Scale;

    [PublicAPI]
    public static class EntityExt
    {
        /// <summary>
        /// Вставка примитива в блок (должна быть запущена транзакция)
        /// </summary>
        /// <param name="ent">Примитив</param>
        /// <param name="btr">Блок</param>
        public static ObjectId Append([NotNull] this Entity ent, [NotNull] BlockTableRecord btr)
        {
            if (!btr.IsWriteEnabled)
                btr.UpgradeOpen();
            var id = btr.AppendEntity(ent);
            btr.Database.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(ent, true);
            return id;
        }

        /// <summary>
        /// Пересечение на плоскости
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="entOther"></param>
        /// <returns>Точки пересечения</returns>
        [NotNull]
        public static List<Point2d> IntersectWithOnPlane([NotNull] this Entity ent, [NotNull] Entity entOther)
        {
            var ptsCol = new Point3dCollection();
            ent.IntersectWith(entOther, Intersect.OnBothOperands, new Plane(), ptsCol, IntPtr.Zero, IntPtr.Zero);
            return ptsCol.Cast<Point3d>().Select(s => s.Convert2d()).ToList();
        }

        /// <summary>
        /// Видим на чертеже, слой включен и разморожен
        /// </summary>
        /// <param name="ent">Объект чертежа</param>
        /// <returns>Да - видим, Нет - не видим, слой выключен или заморожен</returns>
        public static bool IsVisibleLayerOnAndUnfrozen([NotNull] this Entity ent)
        {
            if (!ent.Visible)
                return false;
            using (var lt = (LayerTable)ent.Database.LayerTableId.GetObject(OpenMode.ForRead))
            using (var lay = (LayerTableRecord)lt[ent.Layer].GetObject(OpenMode.ForRead))
            {
                return !lay.IsOff && !lay.IsFrozen; // Слой включен и разморожен  и объект видимый
            }
        }

        /// <summary>
        /// Установка аннотативности объекту и масштаба с удалением текущего масштаба чертежа.
        /// </summary>
        /// <param name="ent">Объект поддерживающий аннотативность (текст, размер и т.п.)</param>
        /// <param name="scale">Масштаб в виде 100, 25 и т.п.</param>
        public static void SetAnnotativeScale([NotNull] this Entity ent, int scale)
        {
            // Проверка, есть ли нужный масштаб в чертеже
            var nameScale = $"1:{scale}";
            var annoScale = ent.Database.GetOrAddAnnotationScale(nameScale, scale, true);
            ent.Annotative = AnnotativeStates.True;
            ent.AddContext(annoScale);
            ent.RemoveContext(ent.Database.Cannoscale);
        }
    }
}