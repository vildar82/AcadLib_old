using System;
using System.IO;
using System.Reflection;
namespace AcadLib
{
   /// <summary>
   /// Загрузка вспомогательных сборок
   /// </summary>
   public static class LoadService
   {
      /// <summary>
      /// Загрузка сборки SpecBlocks.dll - для создания спецификация блоков, в соответствии с настройками.
      /// </summary>
      public static void LoadSpecBlocks()
      {         
         // Загрузка сборки SpecBlocks
         var dllSpecBlocks = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Script\NET\packages\SpecBlocks.dll");
         if (File.Exists(dllSpecBlocks))
         {
            Assembly.LoadFrom(dllSpecBlocks);
         }
         else
         {
            throw new Exception($"Не найден файл {dllSpecBlocks}.");
         }
      }
   }
}