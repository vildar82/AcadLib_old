using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks.Dublicate.Tree;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Blocks.Dublicate
{
   /// <summary>
   /// Проверка наложения блоков в пространстве модели
   /// </summary>
   public static class CheckDublicateBlocks
   {
      public static Tolerance Tolerance { get; set; } = new Tolerance(0.02, 10);
      public static int DEPTH = 5;
      private static int curDepth;
      private static HashSet<ObjectId> attemptedblocks;
      private static List<BlockRefDublicateInfo> AllDublicBlRefInfos;
      private static Dictionary<string, Dictionary<PointTree, List<BlockRefDublicateInfo>>> dictBlRefInfos;

      public static void Check()
      {
         curDepth = 0;
         Database db = HostApplicationServices.WorkingDatabase;
         Inspector.Clear();
         attemptedblocks = new HashSet<ObjectId>();
         AllDublicBlRefInfos = new List<BlockRefDublicateInfo>();
         dictBlRefInfos = new Dictionary<string, Dictionary<PointTree, List<BlockRefDublicateInfo>>>();
         try
         {
            GetDublicateBlocks(SymbolUtilityServices.GetBlockModelSpaceId(db), Matrix3d.Identity, 0);

            // дублирующиеся блоки
            AllDublicBlRefInfos = dictBlRefInfos.SelectMany(s => s.Value.Values).Where(w => w.Count > 1)                                    
                                    .SelectMany(s=>s.GroupBy(g=>g).Where(w=>w.Skip(1).Any()))
                                    .Select(s =>
                                       {
                                          var bi = s.First();
                                          bi.CountDublic = s.Count();
                                          bi.Dublicates = s.Skip(1).ToList();
                                          return bi;
                                       }).ToList();

            // Добавление дубликатов в результирующий список
            //AddTransformedToModelDublic(dublicBlRefInfos);
         }
         catch (Exception ex)
         {
            AutoCAD_PIK_Manager.Log.Error(ex, $"CheckDublicateBlocks - {db.Filename}. {ex.StackTrace}");
            return;
         }

         if (AllDublicBlRefInfos.Count==0)
         {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nДубликаты блоков не найдены.");
         }
         else
         {
            foreach (var dublBlRefInfo in AllDublicBlRefInfos)
            {
               Error err = new Error($"Дублирование блоков '{dublBlRefInfo.Name}' - {dublBlRefInfo.CountDublic} шт. в точке {dublBlRefInfo.Position.ToString()}",
                  dublBlRefInfo.IdBlRef, dublBlRefInfo.TransformToModel, System.Drawing.SystemIcons.Error);
               err.Tag = dublBlRefInfo;
               Inspector.Errors.Add(err);
            }
         }         

         if (Inspector.HasErrors)
         {
            var formDublicates = new FormError(true);
            formDublicates.Text = "Дублирование блоков";
            formDublicates.EnableDublicateButtons();
            if (formDublicates.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
               formDublicates.EnableDialog(false);
               formDublicates.Show();
               throw new Exception("Отменено пользователем.");
            }
         }
      }

      private static void GetDublicateBlocks(ObjectId idBtr, Matrix3d transToModel, double rotate)
      {         
         // Проверялся ли уже такое определение блока
         if (attemptedblocks.Add(idBtr))
         {
            // такой блок еще не проверялся. Перебор его объетов
            List<Tuple<ObjectId, Matrix3d, double>> idsBtrNext = new List<Tuple<ObjectId, Matrix3d, double>>();            
            using (var btr = idBtr.Open(OpenMode.ForRead) as BlockTableRecord)
            {
               // Получение всех вхождений блоков               
               foreach (var idEnt in btr)
               {
                  using (var blRef = idEnt.Open(OpenMode.ForRead, false, true) as BlockReference)
                  {
                     if (blRef == null || !blRef.Visible) continue;
                     BlockRefDublicateInfo blRefInfo = new BlockRefDublicateInfo(blRef, transToModel, rotate);                     

                     Dictionary<PointTree, List<BlockRefDublicateInfo>> dictPointsBlInfos;
                     PointTree ptTree = new PointTree(blRefInfo.Position.X, blRefInfo.Position.Y);

                     if (!dictBlRefInfos.TryGetValue(blRefInfo.Name, out dictPointsBlInfos))
                     {
                        dictPointsBlInfos = new Dictionary<PointTree, List<BlockRefDublicateInfo>>();
                        dictBlRefInfos.Add(blRefInfo.Name, dictPointsBlInfos);
                        
                     }
                     List<BlockRefDublicateInfo> listBiAtPoint;
                     if (!dictPointsBlInfos.TryGetValue(ptTree, out listBiAtPoint))
                     {
                        listBiAtPoint = new List<BlockRefDublicateInfo>();
                        dictPointsBlInfos.Add(ptTree, listBiAtPoint);
                     }
                     listBiAtPoint.Add(blRefInfo);                     

                     idsBtrNext.Add(new Tuple<ObjectId, Matrix3d, double>(item1: blRef.BlockTableRecord, item2: blRef.BlockTransform, item3: blRef.Rotation+rotate));
                  }
               }
            }           

            // Нырок глубже
            if (curDepth < DEPTH)
            {
               curDepth++;
               foreach (var btrNext in idsBtrNext)
               {
                  GetDublicateBlocks(btrNext.Item1, btrNext.Item2, btrNext.Item3);
               }               
            }
         }
      }

      //private static void AddTransformedToModelDublic(List<BlockRefDublicateInfo> dublicBlRefInfos)
      //{
      //   // Трансформированные копии инфоблоков и добавление в результирующий список дубликатов
      //   var trancDublicBlRefInfos = dublicBlRefInfos.Select(b => b.TransCopy()).ToList();
      //   AllDublicBlRefInfos.AddRange(trancDublicBlRefInfos);
      //}

      public static void DeleteDublicates(List<Error> errors)
      {
         if (errors == null || errors.Count == 0)
         {
            return;
         }

         var blDublicatesToDel = errors.Where(e=>e.Tag!=null && e.Tag is BlockRefDublicateInfo).SelectMany(e => ((BlockRefDublicateInfo)e.Tag).Dublicates);
         var doc = Application.DocumentManager.MdiActiveDocument;
         using (doc.LockDocument())
         {
            using (var t = blDublicatesToDel.FirstOrDefault()?.IdBlRef.Database.TransactionManager.StartTransaction())
            {
               foreach (var dublBl in blDublicatesToDel)
               {
                  var blTodel = dublBl.IdBlRef.GetObject(OpenMode.ForWrite, false, true) as BlockReference;
                  blTodel.Erase();
               }
               t.Commit();
            }
         }
         errors.ForEach(e => Inspector.Errors.Remove(e));
      }
   }   
}
