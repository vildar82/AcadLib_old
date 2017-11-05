using System;
using System.Xml.Serialization;
using AcadLib.Colors;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using NetLib;

namespace AcadLib.Layers
{
    [Serializable]
    public class LayerInfo
    {
        private Color color;
        private string colorStr;
        private LineWeight? lineWeight;

        public ObjectId LayerId { get; set; }
        public string Name { get; set; }
        public bool IsOff { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsPlotable { get; set; } = true;
        public bool IsLocked { get; set; }
        [XmlIgnore]
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                colorStr = color.AcadColorToString2();
            }
        }

        public LineWeight LineWeight
        {
            get => lineWeight ?? LineWeight.ByLayer;
            set => lineWeight = value;
        }

        [XmlIgnore]
        public ObjectId LinetypeObjectId { get; set; }
        public string LineType { get; set; }

        /// <summary>
        /// Только для Serializable
        /// </summary>
        public string ColorStr
        {
            get => colorStr;
            set
            {
                colorStr = value;
                color = colorStr.AcadColorFromString2();
            }
        }

        public LayerInfo()
        {
            
        }

        public LayerInfo(string name)
        {
            Name = name;
            Color = Color.FromColorIndex(ColorMethod.ByAci, 7);
            LineWeight = LineWeight.ByLineWeightDefault;
        }

        public LayerInfo(ObjectId idLayer)
        {
            using (var layer = idLayer.Open( OpenMode.ForRead) as LayerTableRecord)
            {
                Name = layer.Name;
                Color = layer.Color;
                using (var lt = layer.LinetypeObjectId.Open(OpenMode.ForRead) as LinetypeTableRecord)
                {
                    LineType = lt.Name;
                }
                LineWeight = layer.LineWeight;
                IsPlotable = layer.IsPlottable;
            }
        }

        /// <summary>
        /// Установка свойст LayerInfo к слою LayerTableRecord
        /// </summary>
        /// <param name="lay"></param>
        public void SetProp(LayerTableRecord lay, Database db)
        {
            if (!Name.IsNullOrEmpty()) lay.Name = Name;
            if (Color != null) lay.Color = Color;
            lay.IsFrozen = IsFrozen;
            lay.IsLocked = IsLocked;
            lay.IsOff = IsOff;
            lay.IsPlottable = IsPlotable;
            if (lineWeight.HasValue) lay.LineWeight = LineWeight;
            if (!LinetypeObjectId.IsNull) lay.LinetypeObjectId = LinetypeObjectId;
            else if (!string.IsNullOrEmpty(LineType)) lay.LinetypeObjectId = db.GetLineTypeIdByName(LineType);
        }
    }
}