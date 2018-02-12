// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 02 12 11:38

using AcadLib.Layers;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using NetLib;

namespace AcadLib.Template
{
    /// <summary>
    /// Управление шаблонами
    /// </summary>
    public class TemplateManager
    {
        public TemplateData Template { get; set; }

        public void ExportToJson([NotNull] string file)
        {
            Template.Serialize(file);
        }

        public void LoadFromDb([NotNull] Database db)
        {
            Template = new TemplateData {Layers = db.Layers()};
        }

        public void LoadFromJson(string file)
        {
            file.Deserialize<TemplateData>();
        }
    }
}