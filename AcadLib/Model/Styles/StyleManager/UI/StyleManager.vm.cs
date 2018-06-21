namespace AcadLib.Styles.StyleManager.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.AutoCAD.DatabaseServices;
    using JetBrains.Annotations;
    using NetLib.WPF;
    using ReactiveUI;

    public class StyleManagerVM : BaseViewModel
    {
        public StyleManagerVM()
        {
            LoadStyleTables();
        }

        public List<StyleTable> StyleTables { get; set; } = new List<StyleTable>();

        private void LoadStyleTables()
        {
            var doc = AcadHelper.Doc;
            var db = doc.Database;
            using (var t = doc.TransactionManager.StartTransaction())
            {
                StyleTables.Add(GetStyleTable(db.TextStyleTableId, "Текстовые стили"));
                StyleTables.Add(GetStyleTable(db.DimStyleTableId, "Размерные стили"));
                StyleTables.Add(GetStyleTableFromDict(db.MLeaderStyleDictionaryId, "Мультивыноски"));
                StyleTables.Add(GetStyleTableFromDict(db.TableStyleDictionaryId, "Стили таблиц"));
                StyleTables.Add(GetStyleTable(db.LinetypeTableId, "Типы линий"));
                StyleTables.Add(GetStyleTable(db.LayerTableId, "Слои"));
                StyleTables.Add(GetStyleTable(db.BlockTableId, "Блоки"));
                t.Commit();
            }
        }

        [NotNull]
        private StyleTable GetStyleTableFromDict(ObjectId styleDictId, string name)
        {
            var dict = styleDictId.GetObjectT<DBDictionary>();
            var styles = dict.Cast<DictionaryEntry>().Select(s => new Style(this)
            {
                Name = s.Key.ToString(),
                Id = (ObjectId)s.Value
            }).OrderBy(o => o.Name);
            return new StyleTable(this)
            {
                Name = name,
                Styles = new ReactiveList<Style>(styles),
                StyleTableId = styleDictId
            };
        }

        [NotNull]
        private StyleTable GetStyleTable(ObjectId symbolTableId, string name)
        {
            var table = symbolTableId.GetObjectT<SymbolTable>();
            var styles = table.GetObjects<SymbolTableRecord>().Select(s => new Style(this)
            {
                Name = s.Name,
                Id = s.Id
            }).OrderBy(o => o.Name);
            return new StyleTable(this)
            {
                Name = name,
                Styles = new ReactiveList<Style>(styles),
                StyleTableId = symbolTableId
            };
        }
    }
}