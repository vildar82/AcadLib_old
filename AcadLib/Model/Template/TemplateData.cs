// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 02 12 14:01

using AcadLib.Layers;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AcadLib.Template
{
    public class TemplateData
    {
        public Dictionary<string, LayerInfo> Layers { get; set; }

        [CanBeNull]
        public LayerInfo GetLayer([NotNull] string layer)
        {
            Layers.TryGetValue(layer, out var li);
            return li;
        }
    }
}