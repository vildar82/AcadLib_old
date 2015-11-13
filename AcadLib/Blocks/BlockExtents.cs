using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Autodesk.AutoCAD.DatabaseServices
{
   public static class BlockExtents
   {
      /// <summary>
      /// Определение границы блока чистых (без динамики, без атрибутов)
      /// </summary>
      /// <param name="blRef"></param>
      /// <returns></returns>
      public static Extents3d GeometricExtentsСlean(this BlockReference blRef)
      {
         Extents3d blockExt = new Extents3d(Point3d.Origin, Point3d.Origin);
         Matrix3d mat = Matrix3d.Identity;
         BlockExtents.GetBlockExtents(blRef, ref blockExt, ref mat);
         return blockExt;
      }

      /// <summary>
      /// Рекурсивное получение габаритного контейнера для выбранного примитива.
      /// </summary>
      /// <param name="en">Имя примитива</param>
      /// <param name="ext">Габаритный контейнер</param>
      /// <param name="mat">Матрица преобразования из системы координат блока в МСК.</param>
      private static void GetBlockExtents(Entity en, ref Extents3d ext, ref Matrix3d mat)
      {
         if (en is BlockReference)
         {
            BlockReference bref = en as BlockReference;
            Matrix3d matIns = mat * bref.BlockTransform;
            using (BlockTableRecord btr =
              bref.BlockTableRecord.Open(OpenMode.ForRead) as BlockTableRecord)
            {
               foreach (ObjectId id in btr)
               {
                  using (DBObject obj = id.Open(OpenMode.ForRead) as DBObject)
                  {
                     Entity enCur = obj as Entity;
                     if (enCur == null || enCur.Visible != true)
                        continue;
                     // Пропускаем определения атрибутов                     
                     if (enCur is AttributeDefinition)
                        continue;
                     GetBlockExtents(enCur, ref ext, ref matIns);
                  }
               }
            }
         }
         else
         {
            if (mat.IsUniscaledOrtho())
            {
               using (Entity enTr = en.GetTransformedCopy(mat))
               {
                  if (enTr is Dimension)
                     (enTr as Dimension).RecomputeDimensionBlock(true);
                  if (enTr is Table)
                     (enTr as Table).RecomputeTableBlock(true);
                  if (IsEmptyExt(ref ext))
                  {
                     try { ext = enTr.GeometricExtents; } catch { };
                  }
                  else
                  {
                     try { ext.AddExtents(enTr.GeometricExtents); } catch { };
                  }
                  return;
               }
            }
            else
            {
               try
               {
                  Extents3d curExt = en.GeometricExtents;
                  curExt.TransformBy(mat);
                  if (IsEmptyExt(ref ext))
                     ext = curExt;
                  else
                     ext.AddExtents(curExt);
               }
               catch { }
               return;
            }
         }
         return;
      }

      /// <summary>
      /// Определят не пустой ли габаритный контейнер.
      /// </summary>
      /// <param name="ext">Габаритный контейнер.</param>
      /// <returns></returns>
      private static bool IsEmptyExt(ref Extents3d ext)
      {
         if (ext.MinPoint.DistanceTo(ext.MaxPoint) < Tolerance.Global.EqualPoint)
            return true;
         else
            return false;
      }
   }
}

