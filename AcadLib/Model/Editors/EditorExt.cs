﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Editors
{
    public static class EditorExt
    {
        /// <summary>
        /// Выделение объектов и зумирование по границе
        /// </summary>
        /// <param name="ids">Элементв</param>
        /// <param name="ed">Редактор</param>
        public static void SetSelectionAndZoom([NotNull] this List<ObjectId> ids, Editor ed)
        {
            try
            {
                if (!ids.Any())
                {
                    "Нет объектов для выделения.".WriteToCommandLine();
                    return;
                }
                ed.SetImpliedSelection(ids.ToArray());
                var ext = new Extents3d();
                ids.Select(s => s.GetObject(OpenMode.ForRead)).Iterate(o =>
                {
                    if (o.Bounds.HasValue) ext.AddExtents(o.Bounds.Value);
                });
                ed.Zoom(ext);
            }
            catch (Exception ex)
            {
                $"Ошибка выделения объектов - {ex.Message}.".WriteToCommandLine();
            }
        }

        public static void AddEntToImpliedSelection(this Editor ed, ObjectId id)
        {
            try
            {
                var idsToSel = new List<ObjectId> { id };
                var selRes = ed.SelectImplied();
                if (selRes.Status == PromptStatus.OK)
                {
                    idsToSel.AddRange(selRes.Value.GetObjectIds());
                }
                ed.SetImpliedSelection(idsToSel.ToArray());
            }
            catch
            {
                //
            }
        }

        /// <summary>
        /// Выбор объектов в заданных границах
        /// В модели
        /// </summary>
        [NotNull]
        public static List<ObjectId> SelectInExtents([NotNull] this Editor ed, Extents3d ext)
        {
            using (ed.Document.LockDocument())
            {
                ed.Document.Database.TileMode = true;
                ed.Zoom(ext);
                var selRes = ed.SelectCrossingWindow(ext.MinPoint, ext.MaxPoint);
                if (selRes.Status == PromptStatus.OK)
                {
                    return selRes.Value.GetObjectIds().ToList();
                }
                throw new OperationCanceledException();
            }
        }

        [NotNull]
        public static List<ObjectId> SelectByPolygon([NotNull] this Editor ed, [NotNull] IEnumerable<Point3d> pts)
        {
            using (ed.Document.LockDocument())
            {
                var selRes = ed.SelectCrossingPolygon(new Point3dCollection(pts.ToArray()));
                if (selRes.Status == PromptStatus.OK)
                {
                    return selRes.Value.GetObjectIds().ToList();
                }
                throw new OperationCanceledException();
            }
        }
    }
}
