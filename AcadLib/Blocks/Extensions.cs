using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using AcDataTable = Autodesk.AutoCAD.DatabaseServices.DataTable;

namespace AcadLib.Blocks
{
   public static class Extensions
   {
      // Opens a DBObject in ForRead mode (kaefer @ TheSwamp)
      public static T GetObject<T>(this ObjectId id) where T : DBObject
      {
         return id.GetObject<T>(OpenMode.ForRead);         
      }

      // Opens a DBObject in the given mode (kaefer @ TheSwamp)
      public static T GetObject<T>(this ObjectId id, OpenMode mode) where T : DBObject
      {
         return id.GetObject(mode) as T;
      }

      // Opens a collection of DBObject in ForRead mode (kaefer @ TheSwamp)       
      public static IEnumerable<T> GetObjects<T>(this IEnumerable ids) where T : DBObject
      {
         return ids.GetObjects<T>(OpenMode.ForRead);
      }

      // Opens a collection of DBObject in the given mode (kaefer @ TheSwamp)
      public static IEnumerable<T> GetObjects<T>(this IEnumerable ids, OpenMode mode) where T : DBObject
      {
         return ids
             .Cast<ObjectId>()
             .Select(id => id.GetObject<T>(mode))
             .Where(res => res != null);
      }

      // Applies the given Action to each element of the collection (mimics the F# Seq.iter function).
      public static void Iterate<T>(this IEnumerable<T> collection, Action<T> action)
      {
         foreach (T item in collection) action(item);
      }

      // Applies the given Action to each element of the collection (mimics the F# Seq.iteri function).
      // The integer passed to the Action indicates the index of element.
      public static void Iterate<T>(this IEnumerable<T> collection, Action<T, int> action)
      {
         int i = 0;
         foreach (T item in collection) action(item, i++);
      }

      // Gets the block effective name (anonymous dynamic blocs).
      public static string GetEffectiveName(this BlockReference br)
      {
         if (br.IsDynamicBlock)
            return br.DynamicBlockTableRecord.GetObject<BlockTableRecord>().Name;
         return br.Name;
      }

      // Creates a System.Data.DataTable from a BlockAttribute collection.
      public static System.Data.DataTable ToDataTable(this IEnumerable<BlockAttribute> blockAtts, string name)
      {
         System.Data.DataTable dTable = new System.Data.DataTable(name);
         dTable.Columns.Add("Name", typeof(string));
         dTable.Columns.Add("Quantity", typeof(int));
         blockAtts
             .GroupBy(blk => blk, (blk, blks) => new { Block = blk, Count = blks.Count() }, new BlockAttributeEqualityComparer())
             .Iterate(row =>
             {
                System.Data.DataRow dRow = dTable.Rows.Add(row.Block.Name, row.Count);
                row.Block.Attributes.Iterate(att =>
                {
                   if (!dTable.Columns.Contains(att.Key))
                      dTable.Columns.Add(att.Key);
                   dRow[att.Key] = att.Value;
                });
             });
         return dTable;
      }

      // Gets the column names collection of the datatable
      public static IEnumerable<string> GetColumnNames(this System.Data.DataTable dataTbl)
      {
         return dataTbl.Columns.Cast<System.Data.DataColumn>().Select(col => col.ColumnName);
      }

      // Writes an Excel file from the datatable (using late binding)
      public static void WriteXls(this System.Data.DataTable dataTbl, string filename, string sheetName, bool visible)
      {
         object mis = Type.Missing;
         object xlApp = LateBinding.GetOrCreateInstance("Excel.Application");
         xlApp.Set("DisplayAlerts", false);
         object workbooks = xlApp.Get("Workbooks");
         object workbook, worksheet;
         if (File.Exists(filename))
            workbook = workbooks.Invoke("Open", filename);
         else
            workbook = workbooks.Invoke("Add", mis);
         if (string.IsNullOrEmpty(sheetName))
            worksheet = workbook.Get("Activesheet");
         else
         {
            object worksheets = workbook.Get("Worksheets");
            try
            {
               worksheet = worksheets.Get("Item", sheetName);
               worksheet.Get("Cells").Invoke("Clear");
            }
            catch
            {
               worksheet = worksheets.Invoke("Add", mis);
               worksheet.Set("Name", sheetName);
            }
         }
         object range = worksheet.Get("Cells");
         dataTbl.GetColumnNames()
             .Iterate((name, i) => range.Get("Item", 1, i + 1).Set("Value2", name));
         dataTbl.Rows
             .Cast<DataRow>()
             .Iterate((row, i) => row.ItemArray
                 .Iterate((item, j) => range.Get("Item", i + 2, j + 1).Set("Value2", item)));
         xlApp.Set("DisplayAlerts", true);
         if (visible)
         {
            xlApp.Set("Visible", true);
         }
         else
         {
            if (File.Exists(filename))
               workbook.Invoke("Save");
            else
            {
               int fileFormat =
                   string.Compare("11.0", (string)xlApp.Get("Version")) < 0 &&
                   filename.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase) ?
                   51 : -4143;
               workbook.Invoke("Saveas", filename, fileFormat, string.Empty, string.Empty, false, false, 1, 1);
            }
            workbook.Invoke("Close");
            workbook = null;
            xlApp.ReleaseInstance();
            xlApp = null;
         }
      }

      // Writes a csv file from the datatable.
      public static void WriteCsv(this System.Data.DataTable dataTbl, string filename)
      {
         using (StreamWriter writer = new StreamWriter(filename))
         {
            writer.WriteLine(dataTbl.GetColumnNames().Aggregate((s1, s2) => string.Format("{0},{1}", s1, s2)));
            dataTbl.Rows
                .Cast<DataRow>()
                .Select(row => row.ItemArray.Aggregate((s1, s2) => string.Format("{0},{1}", s1, s2)))
                .Iterate(line => writer.WriteLine(line));
         }
      }

      // Creates an AutoCAD Table from the datatable.
      public static Table ToAcadTable(this System.Data.DataTable dataTbl, double rowHeight, double columnWidth)
      {
         //return dataTbl.Rows.Cast<DataRow>().ToAcadTable(dataTbl.TableName, dataTbl.GetColumnNames(), rowHeight, columnWidth);
         Table tbl = new Table();
         tbl.Rows[0].Height = rowHeight;
         tbl.Columns[0].Width = columnWidth;
         tbl.InsertColumns(0, columnWidth, dataTbl.Columns.Count - 1);
         tbl.InsertRows(0, rowHeight, dataTbl.Rows.Count + 1);
         tbl.Cells[0, 0].Value = dataTbl.TableName;
         dataTbl.GetColumnNames()
             .Iterate((name, i) => tbl.Cells[1, i].Value = name);
         dataTbl.Rows
             .Cast<DataRow>()
             .Iterate((row, i) =>
                 row.ItemArray.Iterate((item, j) =>
                     tbl.Cells[i + 2, j].Value = item));
         return tbl;
      }
   }
}