using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;

namespace AcadLib.Layers
{
    public class LayerInfo
    {
        public string Name { get; set; }

        public bool IsOff { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsPlotable { get; set; }
        public bool IsLocked { get; set; }
        public Color Color { get; set; }
        public LineWeight LineWeight { get; set; }
        public ObjectId LinetypeObjectId { get; set; }
        public string LineType { get; set; }

        public LayerInfo(string name)
        {
            Name = name;
            Color = Color.FromColorIndex(ColorMethod.ByAci, 7);
            LineWeight = LineWeight.ByLineWeightDefault;
            IsPlotable = true;
        }

        public LayerInfo(ObjectId idLayer)
        {
            using (var layer = idLayer.Open( OpenMode.ForRead)as LayerTableRecord)
            {
                Name = layer.Name;
                Color = layer.Color;
                LineWeight = layer.LineWeight;
                IsPlotable = true;
            }
        }
    }
}