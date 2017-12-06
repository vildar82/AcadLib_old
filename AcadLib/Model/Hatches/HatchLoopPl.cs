using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System;

namespace AcadLib.Hatches
{
    public class HatchLoopPl : IDisposable
    {
        public Curve Loop { get; set; }
        public HatchLoopTypes Types { get; set; }

        public void Dispose()
        {
            Loop?.Dispose();
        }

        [CanBeNull]
        public Polyline GetPolyline()
        {
            switch (Loop)
            {
                case Polyline pl: return pl;
            }
            return null;
        }
    }
}
