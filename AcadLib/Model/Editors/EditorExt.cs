using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcadLib.Editors
{
    public static class EditorExt
    {
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
            }
            throw new Exception($"Ошибка выбора элементов в заданных границах - {ext}");
        }
    }
}
