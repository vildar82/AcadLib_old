using AcadLib.Layers;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace AcadLib.Visual
{
    [PublicAPI]
    public abstract class VisualBase : IVisualService
    {
        protected bool isOn;
        public string LayerForUser { get; set; }

        public bool VisualIsOn
        {
            get => isOn;
            set {
                isOn = value;
                VisualUpdate();
            }
        }

        public VisualBase([CanBeNull] string layer = null)
        {
            LayerForUser = layer ?? SymbolUtilityServices.LayerZeroName;
        }

        public abstract List<Entity> CreateVisual();

        public virtual void Dispose()
        {
            EraseDraws();
        }

        public virtual void VisualsDelete()
        {
            try
            {
                EraseDraws();
            }
            catch
            {
                // ignored
            }
        }

        public virtual void VisualUpdate()
        {
            EraseDraws();
            // Включение визуализации на чертеже
            if (isOn)
            {
                DrawVisuals(CreateVisual());
            }
        }

        protected abstract void DrawVisuals(List<Entity> draws);

        protected abstract void EraseDraws();

        protected ObjectId GetLayerForVisual([CanBeNull] string layer)
        {
            var lay = new LayerInfo(layer ?? SymbolUtilityServices.LayerZeroName);
            return lay.CheckLayerState();
        }
    }
}