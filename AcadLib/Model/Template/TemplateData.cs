// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 02 12 14:01

using AcadLib.Layers;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AcadLib.Template
{
    [PublicAPI]
    public class TemplateData
    {
        public Dictionary<string, LayerInfo> Layers { get; set; } = new Dictionary<string, LayerInfo>();
        public string Name { get; set; }
        private LayerInfo zero = new LayerInfo("0");

        [CanBeNull]
        public LayerInfo GetLayer([NotNull] string layer)
        {
            if (!Layers.TryGetValue(layer, out var li))
            {
                // Нет слоя в шалоне - лог и создать слой
                Logger.Log.Error($"Нет слоя '{layer}' в шаблоне '{Name}'");
                li = new LayerInfo(layer);
                Layers.Add(layer,li);
            }
            return li;
        }
    }
}