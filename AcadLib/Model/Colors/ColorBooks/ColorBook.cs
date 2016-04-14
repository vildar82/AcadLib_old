using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Errors;
using Autodesk.AutoCAD.Colors;
using OfficeOpenXml;

namespace AcadLib.Colors
{
    public class ColorBook
    {
        public string Name { get; set; }
        public List<ColorItem> Colors { get; set; }

        public ColorBook ( string name)
        {
            Name = name;
            Colors = new List<ColorItem>();
        }

        public static ColorBook ReadFromFile(string NcsFile)
        {
            ColorBook colorBookNcs = new ColorBook("NCS");            

            using (var exlPack = new ExcelPackage(new FileInfo(NcsFile)))
            {
                var wsNcs = exlPack.Workbook.Worksheets["NCS"];
                int row = 1;
                do
                {
                    row++;

                    var nameNcs = wsNcs.Cells[row, 1].Text;
                    if (string.IsNullOrEmpty(nameNcs))
                        break;
                    
                    var r = getByte(wsNcs.Cells[row, 2].Text);
                    if (r.Failure)
                    {
                        Inspector.AddError($"Ошибка в ячейке [{row},2] - {r.Error}");
                        continue;
                    }
                    var g = getByte(wsNcs.Cells[row, 3].Text);
                    if (g.Failure)
                    {
                        Inspector.AddError($"Ошибка в ячейке [{row},2] - {g.Error}");
                        continue;
                    }
                    var b = getByte(wsNcs.Cells[row, 4].Text);
                    if (b.Failure)
                    {
                        Inspector.AddError($"Ошибка в ячейке [{row},2] - {b.Error}");
                        continue;
                    }

                    ColorItem colorItem = new ColorItem(nameNcs, r.Value, g.Value, b.Value);
                    colorBookNcs.Colors.Add(colorItem);
                                        
                } while (true);
            }
            return colorBookNcs;
        }

        private static Result<byte> getByte(string value)
        {
            byte res;
            if (!byte.TryParse(value, out res))
            {
                // Ошибка определения бита
                return Result.Fail<byte>($"Не определен бит из значения {value}");
            }
            return Result.Ok(res);
        }
    }
}
