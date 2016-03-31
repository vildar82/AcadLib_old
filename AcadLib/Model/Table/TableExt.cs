using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    public static class TableExt
    {
        public static LineWeight LwDataRow = LineWeight.LineWeight018;

        public static void SetBorders(this Table table, LineWeight lw)
        {
            foreach (var row in table.Rows.Skip(1))
            {
                switch (row.Style)
                {
                    case "_TITLE":
                        setRowTitle(row, LineWeight.LineWeight000);
                        break;
                    case "_HEADER":
                        setRowHeader(row, lw);
                        break;
                    default:
                        setRowData(row, lw);
                        break;
                }
            }
        }

        private static void setRowTitle(Row row, LineWeight lw)
        {
            setCell(row.Borders.Bottom, lw, false);
            setCell(row.Borders.Horizontal, lw, false);
            setCell(row.Borders.Left, lw, false);
            setCell(row.Borders.Right, lw, false);
            setCell(row.Borders.Top, lw, false);
            setCell(row.Borders.Vertical, lw, false);
        }

        private static void setRowHeader(Row row, LineWeight lw)
        {
            setCell(row.Borders.Bottom, lw, true);
            setCell(row.Borders.Horizontal, lw, true);
            setCell(row.Borders.Left, lw, true);
            setCell(row.Borders.Right, lw, true);
            setCell(row.Borders.Top, lw, true);
            setCell(row.Borders.Vertical, lw, true);
        }

        private static void setRowData(Row row, LineWeight lw)
        {
            setCell(row.Borders.Bottom, LwDataRow, true);
            setCell(row.Borders.Horizontal, LwDataRow, true);
            setCell(row.Borders.Left, lw, true);
            setCell(row.Borders.Right, lw, true);
            setCell(row.Borders.Top, LwDataRow, true);
            setCell(row.Borders.Vertical, lw, true);
        }

        private static void setCell(CellBorder cell, LineWeight lw, bool visible)
        {
            cell.LineWeight = lw;
            cell.IsVisible = visible;
        }
    }
}
