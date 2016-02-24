using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Blocks
{
   /// <summary>
   /// Проверка наложения блоков в пространстве модели
   /// </summary>
   public class CheckDublicateBlocks
   {
      public static int DEPTH = 3;
      private int curDepth = 0;

      public CheckDublicateBlocks()
      {

      }

      public void Check()
      {
         Database db = HostApplicationServices.WorkingDatabase;
         Inspector.Clear();
         List<BlockRefInfo> blrefInfos = new List<BlockRefInfo>();

         GetDublicateBlocks(SymbolUtilityServices.GetBlockModelSpaceId(db), ref blrefInfos, Matrix3d.Identity);

         // Выбор будликатов
         var dublicBlRefInfos = blrefInfos.GroupBy(g => g).Where(b => b.Count() > 1);//.SelectMany(s=>s);

         foreach (var dublBlRefInfo in dublicBlRefInfos)
         {
            Inspector.AddError($"Дублирование блоков '{dublBlRefInfo.Key.Name}' - {dublBlRefInfo.Count()} шт. в точке {dublBlRefInfo.Key.Position.ToString()}",
               dublBlRefInfo.Key.IdBlRef, dublBlRefInfo.Key.TransformToModel, System.Drawing.SystemIcons.Error);
         }

         if (Inspector.HasErrors)
         {
            if (Inspector.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
               throw new Exception("Отменено пользователем.");
            }
         }
      }

      private void GetDublicateBlocks(ObjectId idBtr, ref List<BlockRefInfo> blrefInfos, Matrix3d transToModel)
      {
         using (var btr = idBtr.Open(OpenMode.ForRead) as BlockTableRecord)
         {
            // Получение всех блоков
            foreach (var idEnt in btr)
            {
               using (var blRef = idEnt.Open(OpenMode.ForRead, false, true) as BlockReference)
               {
                  if (blRef == null) continue;
                  BlockRefInfo blRefInfo = new BlockRefInfo(blRef, transToModel);
                  blrefInfos.Add(blRefInfo);

                  // Ныряем, но не глубже чем на DEPTH (количество погружений блока в блок)
                  if (curDepth < DEPTH)
                  {
                     curDepth++;
                     GetDublicateBlocks(blRef.BlockTableRecord, ref blrefInfos, transToModel * blRef.BlockTransform);
                  }
               }
            }
         }
      }
   }   
}
