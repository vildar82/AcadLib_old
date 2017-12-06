using AcadLib.Jigs;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using JetBrains.Annotations;
using NetLib;
using System.Linq;

namespace AcadLib.Tables
{
    /// <summary>
    /// Построение таблицы
    /// 1 CalcRows
    /// 2 Create
    /// 3 Insert
    /// </summary>
    public abstract class CreateTable : ICreateTable
    {
        protected readonly Database db;
        protected readonly double scale;

        public LineWeight LwBold { get; set; } = LineWeight.LineWeight050;
        public string Layer { get; set; }
        public int NumRows { get; set; }
        public int NumColumns { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// Имя стиля из шаблона. Если пусто, то ПИК
        /// </summary>
        public string StyleName { get; set; }
        public abstract void CalcRows();
        protected abstract void SetColumnsAndCap(ColumnsCollection columns);
        protected abstract void FillCells(Table table);

        public CreateTable(Database db)
        {
            this.db = db;
            scale = Scale.ScaleHelper.GetCurrentAnnoScale(db);
        }

        public Table Create()
        {
            var table = GetTable();
            return table;
        }

        public void Insert(Table table, Document doc)
        {
            insertTable(table, doc);
        }

        /// <summary>
        /// перед вызовом необходимо заполнить свойства - Title, NumRows, NumColumns
        /// </summary>        
        [NotNull]
        protected Table GetTable()
        {
            var table = new Table();
            table.SetDatabaseDefaults(db);
            table.TableStyle = StyleName.IsNullOrEmpty() ? db.GetTableStylePIK() : db.GetTableStylePIK(StyleName);

            if (!string.IsNullOrEmpty(Layer))
            {
                var layerId = Layers.LayerExt.GetLayerOrCreateNew(new Layers.LayerInfo(Layer));
                table.LayerId = layerId;
            }

            table.SetSize(NumRows, NumColumns);
            TableExt.SetBorders(table, LwBold);
            table.SetRowHeight(8);

            // Название таблицы
            var rowTitle = table.Cells[0, 0];
            rowTitle.Alignment = CellAlignment.MiddleCenter;
            rowTitle.TextHeight = 5;
            rowTitle.TextString = Title;

            // Строка заголовков столбцов
            var rowHeaders = table.Rows[1];
            rowHeaders.Height = 15;
            rowHeaders.Borders.Bottom.LineWeight = LwBold;

            // Заполнение шапки столбцов и их ширин
            SetColumnsAndCap(table.Columns);

            // Заполнение строк
            FillCells(table);

            var lastRow = table.Rows.Last();
            lastRow.Borders.Bottom.LineWeight = LwBold;

            table.GenerateLayout();
            return table;
        }

        private void insertTable(Table table, [NotNull] Document doc)
        {
            using (var t = doc.TransactionManager.StartTransaction())
            {
                var jigTable = new TableJig(table, scale, "Вставка таблицы");
                if (doc.Editor.Drag(jigTable).Status == PromptStatus.OK)
                {
                    var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                    cs.AppendEntity(table);
                    t.AddNewlyCreatedDBObject(table, true);
                }
                t.Commit();
            }
        }
    }
}
