using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Blocks
{
   /// <summary>
   /// Проверка наложения блоков в пространстве модели
   /// </summary>
   public class CheckDublicateBlocks
   {
      public CheckDublicateBlocks()
      {

      }

      public void Check()
      {
         Database db = HostApplicationServices.WorkingDatabase;
         using (var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).Open( OpenMode.ForRead) as BlockTableRecord)
         {
            Inspector.Clear();
            List<BlockRefInfo> blrefInfos = new List<BlockRefInfo>();
            // Получение всех блоков
            foreach (var idEnt in ms)
            {
               using (var blRef = idEnt.Open( OpenMode.ForRead, false, true)as BlockReference)
               {
                  if (blRef == null) continue;
                  BlockRefInfo blRefInfo = new BlockRefInfo(blRef);
                  blrefInfos.Add(blRefInfo);
               }
            }
            // Выбор будликатов
            var dublicBlRefInfos = blrefInfos.GroupBy(g => g).Where(b => b.Count() > 1);//.SelectMany(s=>s);

            foreach (var dublBlRefInfo in dublicBlRefInfos)
            {
               Inspector.AddError($"Дублирование блоков '{dublBlRefInfo.Key.Name}' - {dublBlRefInfo.Count()} шт. в точке {dublBlRefInfo.Key.Position.ToString()}",
                  dublBlRefInfo.Key.IdBlRef, System.Drawing.SystemIcons.Error);
            }

            if (Inspector.HasErrors)
            {
               if (Inspector.ShowDialog() != System.Windows.Forms.DialogResult.OK)
               {
                  throw new Exception("Отменено пользователем.");
               }
            }
         }
      }
   }   
}
