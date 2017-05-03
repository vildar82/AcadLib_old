using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

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

        /// <summary>
        /// Установка свойст LayerInfo к слою LayerTableRecord
        /// </summary>
        /// <param name="lay"></param>
        public void SetProp(LayerTableRecord lay, Database db)
        {
            lay.Name = Name;
            lay.Color = Color;
            lay.IsFrozen = IsFrozen;
            lay.IsLocked = IsLocked;
            lay.IsOff = IsOff;
            lay.IsPlottable = IsPlotable;
            lay.LineWeight = LineWeight;
            if (!LinetypeObjectId.IsNull)
                lay.LinetypeObjectId = LinetypeObjectId;
            else if (!string.IsNullOrEmpty(LineType))
            {
                lay.LinetypeObjectId = db.GetLineTypeIdByName(LineType);
            }
            else
            {
                lay.LinetypeObjectId = db.GetLineTypeIdContinuous();
            }
        }
    }
}