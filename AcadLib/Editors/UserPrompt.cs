using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace Autodesk.AutoCAD.EditorInput
{
   public static class UserPrompt
   {
      public static Extents3d PromptExtents(this Editor ed, string msgPromptFirstPoint, string msgPromptsecondPoint)
      {
         Extents3d extentsPrompted = new Extents3d();
         var prPtRes = ed.GetPoint(msgPromptFirstPoint);
         if (prPtRes.Status == PromptStatus.OK)
         {
            var prCornerRes = ed.GetCorner(msgPromptsecondPoint, prPtRes.Value);
            if (prCornerRes.Status == PromptStatus.OK)
            {
               extentsPrompted.AddPoint(prPtRes.Value);
               extentsPrompted.AddPoint(prCornerRes.Value);
            }
            else
            {
               throw new Exception("Отменено пользователем.");
            }
         }
         else
         {
            throw new Exception("Отменено пользователем.");
         }
         return extentsPrompted;
      }

      /// <summary>
      /// Простой запрос точки - с преобразованием в WCS
      /// </summary>      
      /// <param name="msg">Строка запроса</param>
      /// <exception cref="Exception">Отменено пользователем.</exception>
      /// <returns>Point3d in WCS</returns>
      public static Point3d GetPointWCS(this Editor ed, string msg)
      {                  
         var res = ed.GetPoint(msg);
         if (res.Status == PromptStatus.OK)
         {
            return res.Value.TransformBy(ed.CurrentUserCoordinateSystem);
         }
         else
         {
            throw new Exception("Отменено пользователем.");
         }
      }

      /// <summary>
      /// Запрос целого числа
      /// </summary>      
      /// <param name="defaultNumber">Номер по умолчанию</param>
      /// <param name="msg">Строка запроса</param>
      /// <exception cref="Exception">Отменено пользователем.</exception>
      /// <returns>Результат успешного запроса.</returns>
      public static int GetNumber(this Editor ed, int defaultNumber, string msg)
      {
         var opt = new PromptIntegerOptions(msg);         
         opt.DefaultValue = defaultNumber;
         var res = ed.GetInteger(opt);
         if (res.Status == PromptStatus.OK)
         {
            return res.Value;
         }
         else
         {
            throw new Exception("Отменено пользователем.");
         }
      }

      /// <summary>
      /// Pапрос выбора объектов
      /// </summary>      
      /// <param name="msg">Строка запроса</param>
      /// <exception cref="Exception">Отменено пользователем.</exception>
      /// <returns>Список выбранных объектов</returns>
      public static List<ObjectId> Select(this Editor ed,  string msg)
      {
         var selOpt = new PromptSelectionOptions();         
         selOpt.MessageForAdding = msg;         
         var selRes = ed.GetSelection(selOpt);
         if (selRes.Status == PromptStatus.OK)
         {
            return selRes.Value.GetObjectIds().ToList();
         }
         else
         {
            throw new Exception("\nОтменено пользователем");
         }
      }
   }
}
