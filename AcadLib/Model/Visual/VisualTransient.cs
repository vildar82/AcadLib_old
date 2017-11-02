using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;

namespace AcadLib.Visual
{
    /// <summary>
    /// Визуализация графики - через TransientManager
    /// </summary>
    public abstract class VisualTransient : VisualBase
    {
	    public static readonly Autodesk.AutoCAD.Geometry.IntegerCollection vps = new Autodesk.AutoCAD.Geometry.IntegerCollection();        
        protected List<Entity> draws;

        public VisualTransient(string layer = null) : base(layer)
        {
        }

        /// <summary>
        /// Включение/отключение визуализации (без перестроений)
        /// </summary>
        protected override void DrawVisuals(List<Entity> draws)
        {
            this.draws = draws;
            if (draws != null)
            {
                var tm = TransientManager.CurrentTransientManager;
                foreach (var d in draws)
                {
                    tm.AddTransient(d, TransientDrawingMode.Main, 0, vps);
                }
            }
        }

        protected override void EraseDraws ()
        {
            if (draws == null || draws.Count == 0) return;
            var tm = TransientManager.CurrentTransientManager;
            foreach (var item in draws)
            {
                tm.EraseTransient(item, vps);
                item.Dispose();
            }
            draws = null;
        }

        private void DisposeDraws()
        {
            if (draws?.Any() != true) return;
            foreach (var draw in draws)
            {
                draw?.Dispose();
            }
        }

        public static void EraseAll()
        {
            try
            {
                TransientManager.CurrentTransientManager.EraseTransients(TransientDrawingMode.Main, 0, vps);
            }
            catch { }
        }
    }
}
