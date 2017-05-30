using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal.DatabaseServices;

namespace AcadLib.Blocks
{
	/// <summary>
	/// Таблица свойств блока
	/// </summary>
	public static class BlockPropertiesTableExt
	{
		/// <summary>
		/// Таблица свойств блока. Должна быть запущена транзакция!
		/// </summary>
		public static System.Data.DataTable GetBlockPropertiesTable(this BlockTableRecord dynBtr)
		{
			var t = dynBtr.Database.TransactionManager.TopTransaction;
			var bpt = GetBPT(dynBtr, t);
			var dTable = new System.Data.DataTable($"Таблица свойств блока {dynBtr.Name}");
			dTable.Columns.AddRange(GetColumns(bpt).ToArray());
			foreach (BlockPropertiesTableRow bptRow in bpt.Rows)
			{
				var row = dTable.NewRow();
				foreach (BlockPropertiesTableColumn bptCol in bpt.Columns)
				{
					var tv = bptRow[bptCol];
					var val = tv.AsArray()[0].Value;
					row[bptCol.Parameter.Name] = val;
				}
				dTable.Rows.Add(row);
			}
			return dTable;
		}

		private static List<System.Data.DataColumn> GetColumns(BlockPropertiesTable bpt)
		{
			var columns = new List<System.Data.DataColumn>();
			foreach (BlockPropertiesTableColumn bptColumn in bpt.Columns)
			{
				var type = bptColumn.DefaultValue.AsArray()[0].Value.GetType();
				var col = new System.Data.DataColumn(bptColumn.Parameter.Name, type);
				columns.Add(col);
			}
			return columns;
		}

		private static BlockPropertiesTable GetBPT(BlockTableRecord dynBtr, Transaction t)
		{
			var extDic = dynBtr.ExtensionDictionary.GetObject<DBDictionary>();
			var graph = extDic.GetAt("ACAD_ENHANCEDBLOCK").GetObject<EvalGraph>();
		 	return graph.GetAllNodes()
				.Select(f=> graph.GetNode((uint)f, OpenMode.ForRead, t) as BlockPropertiesTable)
				.FirstOrDefault(w => w!=null);
		}
	}
}