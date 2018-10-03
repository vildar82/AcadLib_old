namespace AcadLib.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Geometry;
    using JetBrains.Annotations;
    using NetLib.Monad;

    public static class EditorExt
    {
        public static void AcadLoadInfo([NotNull] this Assembly assm)
        {
            try
            {
                var asmName = assm.GetName();
                var msg = $"PIK. {asmName.Name} загружен, версия {asmName.Version}";
                msg.WriteToCommandLine();
                Logger.Log.Info(msg);
            }
            catch
            {
                //
            }
        }

        public static void AcadLoadError(
            [NotNull] this Assembly assm,
            [CanBeNull] Exception ex = null,
            [CanBeNull] string err = null)
        {
            try
            {
                var asmName = assm.GetName();
                $"PIK. Ошибка загрузки {asmName.Name}, версия:{asmName.Version} - {err} {ex?.Message}.".WriteToCommandLine();
            }
            catch
            {
                //
            }
        }

        /// <summary>
        /// Выделение объектов и зумирование по границе
        /// </summary>
        /// <param name="ids">Элементв</param>
        /// <param name="ed">Редактор</param>
        public static void SetSelectionAndZoom([NotNull] this List<ObjectId> ids, Editor ed = null)
        {
            try
            {
                var doc = AcadHelper.Doc;
                ed = doc.Editor;
                using (doc.LockDocument())
                using (var t = doc.TransactionManager.StartTransaction())
                {
                    if (!ids.Any())
                    {
                        "Нет объектов для выделения.".WriteToCommandLine();
                        return;
                    }

                    var ext = new Extents3d();
                    ids.Select(s => s.GetObject(OpenMode.ForRead)).Iterate(o =>
                    {
                        if (o.Bounds.HasValue)
                            ext.AddExtents(o.Bounds.Value);
                    });
                    ed.Zoom(ext);
                    ed.SetImpliedSelection(ids.ToArray());
                    t.Commit();
                }
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
                ed.Try(e => e.Document.Database.TileMode = true);
                ed.Try(e => e.Zoom(ext));
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