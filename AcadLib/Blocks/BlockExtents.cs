using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace Autodesk.AutoCAD.DatabaseServices
{
   public static class BlockExtents
   {
      public static RXClass DbTextRXClass = RXClass.GetClass(typeof(DBText));
      public static RXClass MTextRXClass = RXClass.GetClass(typeof(MText));
      public static RXClass MLeaderRXClass = RXClass.GetClass(typeof(MLeader));
      public static RXClass DimensionRXClass = RXClass.GetClass(typeof(Dimension));

      /// <summary>
      /// Определение границы блока чистых (без динамики, без атрибутов)
      /// </summary>
      /// <param name="blRef"></param>
      /// <returns></returns>
      public static Extents3d GeometricExtentsСlean(this BlockReference blRef)
      {         
         Matrix3d mat = Matrix3d.Identity;
         Extents3d blockExt = BlockExtents.GetBlockExtents(blRef, ref mat, new Extents3d ());
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
         if (en is BlockReference)
         {
            var bref = en as BlockReference;
            Matrix3d matIns = mat * bref.BlockTransform;
            using (var btr = bref.BlockTableRecord.Open(OpenMode.ForRead) as BlockTableRecord)
            {
               foreach (ObjectId id in btr)
               {
                  // Пропускаем все тексты.
                  if (id.ObjectClass.IsDerivedFrom(DbTextRXClass) ||
                     id.ObjectClass.IsDerivedFrom(MTextRXClass) ||
                     id.ObjectClass.IsDerivedFrom(MLeaderRXClass) ||
                     id.ObjectClass.IsDerivedFrom(DimensionRXClass))
                  {
                     continue;
                  }
                  using (DBObject obj = id.Open(OpenMode.ForRead) as DBObject)
                  {
                     Entity enCur = obj as Entity;
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
                  return ext;
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
      private static bool IsEmptyExt(ref Extents3d ext)
      {
         if (ext.MinPoint.DistanceTo(ext.MaxPoint) < Tolerance.Global.EqualPoint)
            return true;
         else
            return false;
      }
   }
}

