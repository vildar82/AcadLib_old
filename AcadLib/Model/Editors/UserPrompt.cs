using System;
using System.Collections.Generic;
using System.Linq;
using AcadLib.Jigs;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using AcadLib;

namespace Autodesk.AutoCAD.EditorInput
{
    public static class UserPrompt
    {
        /// <summary>
        /// Выбор объекта на чертеже заданного типа
        /// </summary>
        /// <typeparam name="T">Тип выбираемого объекта</typeparam>
        /// <param name="ed">Editor</param>
        /// <param name="prompt">Строка запроса</param>
        /// <param name="rejectMsg">Сообщение при выбора неправильного типа объекта</param>
        /// <returns>Выбранный объект</returns>
        public static ObjectId SelectEntity<T>(this Editor ed, string prompt, string rejectMsg) where T : Entity
        {
            var selOpt = new PromptEntityOptions($"\n{prompt}");
            selOpt.SetRejectMessage($"\n{rejectMsg}");
            selOpt.AddAllowedClass(typeof(T), true);            
            var selRes = ed.GetEntity(selOpt);
            if (selRes.Status != PromptStatus.OK)
            {
                throw new CancelByUserException();
            }
            return selRes.ObjectId;
        }

        public static Extents3d PromptExtents(this Editor ed, string msgPromptFirstPoint, string msgPromptsecondPoint)
        {
            var extentsPrompted = new Extents3d();
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
                    throw new CancelByUserException();
                }
            }
            else
            {
                throw new CancelByUserException();
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
                throw new CancelByUserException();
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
            var opt = new PromptIntegerOptions(msg)
            {
                DefaultValue = defaultNumber
            };
            var res = ed.GetInteger(opt);
            if (res.Status == PromptStatus.OK)
            {
                return res.Value;
            }
            else
            {
                throw new CancelByUserException();
            }
        }

        /// <summary>
        /// Pапрос выбора объектов
        /// </summary>      
        /// <param name="msg">Строка запроса</param>
        /// <exception cref="Exception">Отменено пользователем.</exception>
        /// <returns>Список выбранных объектов</returns>
        public static List<ObjectId> Select(this Editor ed, string msg)
        {
            var selOpt = new PromptSelectionOptions()
            {
                MessageForAdding = msg
            };
            var selRes = ed.GetSelection(selOpt);
            if (selRes.Status == PromptStatus.OK)
            {
                return selRes.Value.GetObjectIds().ToList();
            }
            else
            {
                throw new CancelByUserException();
            }
        }

        /// <summary>
        /// Pапрос выбора блоков
        /// </summary>      
        /// <param name="msg">Строка запроса</param>
        /// <exception cref="Exception">Отменено пользователем.</exception>
        /// <returns>Список выбранных блоков</returns>
        public static List<ObjectId> SelectBlRefs(this Editor ed, string msg)
        {
            var filList = new TypedValue[1] { new TypedValue((int)DxfCode.Start, "INSERT") };
            var filter = new SelectionFilter(filList);
            var selOpt = new PromptSelectionOptions()
            {
                MessageForAdding = msg
            };
            var selRes = ed.GetSelection(selOpt, filter);
            if (selRes.Status == PromptStatus.OK)
            {
                return selRes.Value.GetObjectIds().ToList();
            }
            else
            {
                throw new CancelByUserException();
            }
        }

        /// <summary>
        /// Запрос точки вставки с висящим прямоугольником на курсоре - габариты всталяемого объекта
        /// Чтобы человек выбрал нашел место на чертежа соответствующих размеров.
        /// Точка - нижний левый угол
        /// </summary>                
        public static Point3d GetRectanglePoint(this Editor ed, double len, double height)
        {
            var jigRect = new RectangleJig(len, height);            
            var res = ed.Drag(jigRect);
            if (res.Status != PromptStatus.OK)
                throw new Exception(General.CanceledByUser);
            return jigRect.Position;
        }
    }
}