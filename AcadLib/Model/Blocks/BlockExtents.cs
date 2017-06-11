using AcadLib;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace Autodesk.AutoCAD.DatabaseServices
{
    public static class BlockExtents
    {
        

        /// <summary>
        /// Обновление графики во вхождениях блока для данного определения блока
        /// Должна быть запущена транзакция!!!
        /// </summary>        
        public static void SetBlRefsRecordGraphicsModified(this BlockTableRecord btr)
        {
            var idsBlRef = btr.GetBlockReferenceIds(true, false);
            foreach (ObjectId idBlRefApart in idsBlRef)
            {
                var blRefApartItem = idBlRefApart.GetObject(OpenMode.ForWrite, false, true) as BlockReference;
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
				using (var btr = bref.BlockTableRecord.Open(OpenMode.ForRead) as BlockTableRecord)
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
						using (var obj = id.Open(OpenMode.ForRead))
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
							try { ext = enTr.GeometricExtents; } catch { };
						}
						else
						{
							try { ext.AddExtents(enTr.GeometricExtents); } catch { };
						}
						return ext;
					}
				}
				else
				{
					try
					{
						var curExt = en.GeometricExtents;
						curExt.TransformBy(mat);
						if (IsEmptyExt(ref ext))
							ext = curExt;
						else
							ext.AddExtents(curExt);
					}
					catch { }
					return ext;
				}
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