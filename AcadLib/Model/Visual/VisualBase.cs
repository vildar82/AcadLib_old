using AcadLib.Layers;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace AcadLib.Visual
{ 
	public abstract class VisualBase : IVisualService
    {        
        protected bool isOn;                
        public string LayerForUser { get; set; }         

        public VisualBase(string layer = null)
        {
            LayerForUser = layer ?? SymbolUtilityServices.LayerZeroName;
        }

        public abstract List<Entity> CreateVisual();        
        protected abstract void DrawVisuals(List<Entity> draws);
        protected abstract void EraseDraws();

        public bool VisualIsOn {
            get => isOn;
	        set 
			{
                isOn = value;
                VisualUpdate();
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

        public virtual void VisualsDelete()
        {
            try
            {
                EraseDraws();
            }
            catch { }
        }

        protected ObjectId GetLayerForVisual(string layer)
        {
            var lay = new LayerInfo(layer ?? SymbolUtilityServices.LayerZeroName);
            return lay.CheckLayerState();
        }

        public virtual void Dispose()
        {
            EraseDraws();
        }
    }
}
