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
            if (table.Rows.Count < 2) return;

            var rowTitle = table.Rows[0];
            setRowTitle(rowTitle);

            var rowHead = table.Rows[1];
            setRowHeader(rowHead, lw);

            foreach (var row in table.Rows.Skip(2))
            {
                setRowData(row, lw);
            }
        }

        private static void setRowTitle(Row row)
        {
            setCell(row.Borders.Bottom,  LineWeight.LineWeight000, false);
            setCell(row.Borders.Horizontal, LineWeight.LineWeight000, false);
            setCell(row.Borders.Left, LineWeight.LineWeight000, false);
            setCell(row.Borders.Right, LineWeight.LineWeight000, false);
            setCell(row.Borders.Top, LineWeight.LineWeight000, false);
            setCell(row.Borders.Vertical, LineWeight.LineWeight000, false);
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
            //cell.Overrides = GridProperties.Visibility | GridProperties.LineWeight;
            cell.LineWeight = lw;
            cell.IsVisible = visible;            
        }
    }
}
