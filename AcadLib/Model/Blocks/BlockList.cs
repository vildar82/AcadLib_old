using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Microsoft.Office.Interop.Excel;

namespace AcadLib
{
    public static class BlockList
    {
        public static void List(this Database db)
        {
            List<string> list = new List<string>();
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
                var excelApp = new Microsoft.Office.Interop.Excel.Application { DisplayAlerts = false };
                if (excelApp == null) return;

                // Открываем книгу
                Workbook workBook = excelApp.Workbooks.Add();
                Worksheet sheet = workBook.ActiveSheet as Worksheet;
                sheet.Name = "Блоки";
                sheet.Cells[1, 1].Value = $"{db.Filename}, {DateTime.Now}";

                sheet.Cells[2, 1].Value = "№пп";
                sheet.Cells[2, 2].Value = "Имя";

                int row = 3;
                int count = 1;
                foreach (var name in list)
                {
                    sheet.Cells[row, 1].Value = count.ToString();
                    count++;
                    sheet.Cells[row, 2].Value = name;
                    row++;
                }
                excelApp.Visible = true;
            }            
        }
    }
}
