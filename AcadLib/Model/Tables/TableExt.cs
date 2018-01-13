using AcadLib.Geometry;
using AcadLib.Hatches;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JetBrains.Annotations;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace AcadLib
{
    [PublicAPI]
    public static class TableExt
    {
        public static LineWeight LwDataRow = LineWeight.LineWeight018;

        public static void SetBorders([NotNull] this Table table, LineWeight lw)
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

        /// <summary>
        /// Вставка штриховки в ячеку таблицы.
        /// Должна быть запущена транзакция.
        /// Таблица должна быть в базе чертежа.
        /// Штриховка добавляется в базу.
        /// </summary>
        [NotNull]
        public static Hatch SetCellHatch([NotNull] this Cell cell, int colorIndex = 0, LineWeight lineWeight = LineWeight.LineWeight015,
            double patternScale = 1, string standartPattern = "LINE", double patternAngleRad = 0)
        {
            var table = cell.ParentTable;
            table.RecomputeTableBlock(true);
            var btr = (BlockTableRecord)table.OwnerId.GetObject(OpenMode.ForWrite);
            var cellExt = OffsetExtToMarginCell(cell.GetExtents().ToExtents3d(), cell);
            using (var cellPl = cellExt.GetPolyline())
            {
                var h = cellPl.GetPoints().CreateHatch();
                h.PatternAngle = patternAngleRad;
                h.PatternScale = patternScale;
                h.SetHatchPattern(HatchPatternType.PreDefined, standartPattern);
                h.ColorIndex = colorIndex;
                h.LineWeight = lineWeight;
                h.Linetype = SymbolUtilityServices.LinetypeContinuousName;
                var t = btr.Database.TransactionManager.TopTransaction;
                btr.AppendEntity(h);
                t.AddNewlyCreatedDBObject(h, true);
                h.EvaluateHatch(true);
                return h;
            }
        }

        private static Extents3d OffsetExtToMarginCell(Extents3d ext, [NotNull] Cell cell)
        {
            return new Extents3d(
                new Point3d(ext.MinPoint.X - cell.Borders.Horizontal.Margin ?? 0, ext.MinPoint.Y - cell.Borders.Top.Margin ?? 0, 0),
                new Point3d(ext.MaxPoint.X + cell.Borders.Horizontal.Margin ?? 0, ext.MaxPoint.Y + cell.Borders.Top.Margin ?? 0, 0));
        }

        private static void setCell([NotNull] CellBorder cell, LineWeight lw, bool visible)
        {
            //cell.Overrides = GridProperties.Visibility | GridProperties.LineWeight;
            cell.LineWeight = lw;
            cell.IsVisible = visible;
        }

        private static void setRowData([NotNull] Row row, LineWeight lw)
        {
            setCell(row.Borders.Bottom, LwDataRow, true);
            setCell(row.Borders.Horizontal, LwDataRow, true);
            setCell(row.Borders.Left, lw, true);
            setCell(row.Borders.Right, lw, true);
            setCell(row.Borders.Top, LwDataRow, true);
            setCell(row.Borders.Vertical, lw, true);
        }

        private static void setRowHeader([NotNull] Row row, LineWeight lw)
        {
            setCell(row.Borders.Bottom, lw, true);
            setCell(row.Borders.Horizontal, lw, true);
            setCell(row.Borders.Left, lw, true);
            setCell(row.Borders.Right, lw, true);
            setCell(row.Borders.Top, lw, true);
            setCell(row.Borders.Vertical, lw, true);
        }

        private static void setRowTitle([NotNull] Row row)
        {
            setCell(row.Borders.Bottom, LineWeight.LineWeight000, false);
            setCell(row.Borders.Horizontal, LineWeight.LineWeight000, false);
            setCell(row.Borders.Left, LineWeight.LineWeight000, false);
            setCell(row.Borders.Right, LineWeight.LineWeight000, false);
            setCell(row.Borders.Top, LineWeight.LineWeight000, false);
            setCell(row.Borders.Vertical, LineWeight.LineWeight000, false);
        }
    }
}