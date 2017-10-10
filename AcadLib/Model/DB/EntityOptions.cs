using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    /// <summary>
    /// Настройки для объекта на чертеже
    /// </summary>
    public class EntityOptions
    {
        private LineWeight lineWeight;
        private bool isLineWeight;
        private int colorIndex;
        private bool isColorIndex;

        public ObjectId LayerId { get; set; }
        public string Layer { get; set; }
        public ObjectId LineTypeId { get; set; }
        public string LineType { get; set; }
        public double? LinetypeScale { get; set; }
        public LineWeight LineWeight
        {
            get => lineWeight;
            set { lineWeight = value; isLineWeight = true; }
        }
        public int ColorIndex
        {
            get => colorIndex;
            set { colorIndex = value; isColorIndex = true; }
        }
        public System.Drawing.Color Color { get; set; }
        public Autodesk.AutoCAD.Colors.Color AcadColor { get; set; }
        /// <summary>
        /// Создавать или копировать из шаблона отсутствующие значения в чертеже. 
        /// </summary>
        public bool CheckCreateValues { get; set; }

        public EntityOptions()
        {   
        }

        public void SetOptions(Entity ent)
        {
            if (!ent.IsWriteEnabled)
            {
                ent.UpgradeOpen();
            }
            SetLayer(ent);
            SetColor(ent);
            SetLineWeight(ent);
            SetLineType(ent);
            SetLinetypeScale(ent);
        }

        private void SetLinetypeScale(Entity ent)
        {
            if (LinetypeScale != null && LinetypeScale.Value >0)
            {
                ent.LinetypeScale = LinetypeScale.Value;
            }
        }

        public void SetLineType (Entity ent)
        {
            if (!LineTypeId.IsNull)
            {
                ent.LinetypeId = LineTypeId;                
            }
            else if (!string.IsNullOrEmpty(LineType))
            {
                if (CheckCreateValues)
                {
                    ent.Database.LoadLineTypePIK(LineType);
                }
                ent.Linetype = LineType;
            }
        }

        public void SetLineWeight (Entity ent)
        {
            if (isLineWeight)
            {
                ent.LineWeight = LineWeight;
            }
        }

        public void SetColor (Entity ent)
        {
            if (isColorIndex)
            {
                ent.ColorIndex = ColorIndex;
            }
            else if (!Color.IsEmpty)
            {
                ent.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color);                
            }
            else if (AcadColor != null)
            {
                ent.Color = AcadColor;
            }
        }

        public void SetLayer (Entity ent)
        {
            if (!LayerId.IsNull)
            {
                ent.LayerId = LayerId;                
            }
            else if (!string.IsNullOrEmpty(Layer))
            {
                if (CheckCreateValues)
                {
                    Layers.LayerExt.CheckLayerState(Layer);
                }
                ent.Layer = Layer;
            }
        }
    }

    public static class EntityOptionsExt
    {
        public static void SetOptions(this Entity ent, EntityOptions opt)
        {
            opt?.SetOptions(ent);
        }
    }
}
