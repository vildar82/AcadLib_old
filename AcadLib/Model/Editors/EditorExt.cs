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
