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
        public Dictionary<string, LayerInfo> Layers { get; set; }
        public string Name { get; set; }
        private LayerInfo zero = new LayerInfo("0");

        [CanBeNull]
        public LayerInfo GetLayer([NotNull] string layer)
        {
            if (Layers.TryGetValue(layer, out var li))
            {
                return li;
            }
            // Нет слоя в шалоне - лог и вернуть текущий слой
            Logger.Log.Error($"Нет слоя '{layer}' в шаблоне '{Name}'");
            Layers.Add(layer, zero);
            return zero;
        }
    }
}