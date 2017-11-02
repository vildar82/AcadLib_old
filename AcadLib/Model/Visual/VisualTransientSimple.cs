using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Visual
{
    public class VisualTransientSimple : VisualTransient
    {
        private readonly List<Entity> ents;

        public VisualTransientSimple(List<Entity> ents)
        {
            this.ents = ents;
        }

        public override List<Entity> CreateVisual()
        {
            return ents;
        }
    }
}
