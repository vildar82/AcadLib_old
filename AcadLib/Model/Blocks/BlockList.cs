using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Autodesk.AutoCAD.DatabaseServices;
using OfficeOpenXml;
using Path = NetLib.IO.Path;

namespace AcadLib
{
    public static class BlockList
    {
        public static void List(this Database db)
        {
            var list = new List<string>();
            using (var t = db.TransactionManager.StartTransaction())
            {
                var bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                foreach (var item in bt)
                {
                    var btr = item.GetObject(OpenMode.ForRead) as BlockTableRecord;
                    if (btr != null && !btr.IsLayout && !btr.IsAnonymous && !btr.IsDependent)
                    {
                        list.Add(btr.Name);
                    }
                }
                t.Commit();
            }
            // Запись в Excel
            if (list.Count > 0)
            {
                var tempFile = new FileInfo(Path.GetTempFile(".xlsx"));
                using (var excel = new ExcelPackage(tempFile))
                {
                    // Открываем книгу
                    var sheet = excel.Workbook.Worksheets.Add("Блоки");
                    sheet.Cells[1, 1].Value = $"{db.Filename}, {DateTime.Now}";
                    sheet.Cells[2, 1].Value = "№пп";
                    sheet.Cells[2, 2].Value = "Имя";
                    var row = 3;
                    var count = 1;
                    foreach (var name in list)
                    {
                        sheet.Cells[row, 1].Value = count.ToString();
                        count++;
                        sheet.Cells[row, 2].Value = name;
                        row++;
                    }
                    excel.Save();
                }
                Process.Start(tempFile.FullName);
            }            
        }
    }
}
