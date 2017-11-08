using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace AcadLib.Editors
{
	public static class EditorExt
	{
	    public static void AddEntToImpliedSelection(this Editor ed, ObjectId id)
	    {
	        try
	        {
	            var idsToSel = new List<ObjectId> {id};
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
        public static List<ObjectId> SelectInExtents(this Editor ed, Extents3d ext)
		{
		    ed.Document.Database.TileMode = true;
			ed.Zoom(ext);
			var selRes = ed.SelectCrossingWindow(ext.MinPoint, ext.MaxPoint);
			if (selRes.Status == PromptStatus.OK)
			{
				return selRes.Value.GetObjectIds().ToList();
			}
			throw new Exception($"Ошибка выбора элементов в заданных границах - {ext}");
		}
	}
}
