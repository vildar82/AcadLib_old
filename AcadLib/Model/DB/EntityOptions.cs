using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    /// <summary>
    /// Настройки для объекта на чертеже
    /// </summary>
    public class EntityOptions
    {
        public ObjectId LayerId { get; set; }
        public string Layer { get; set; }
        public ObjectId LineTypeId { get; set; }
        public string LineType { get; set; }
        public LineWeight LineWeight { get { return lineWeight; } set { lineWeight = value; isLineWeight = true; } }
        LineWeight lineWeight;
        bool isLineWeight;

        public int ColorIndex { get { return colorIndex; } set { colorIndex = value; isColorIndex = true; } }
        int colorIndex;
        bool isColorIndex;

        public System.Drawing.Color Color { get; set; }
        
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
        }

        public void SetLineType (Entity ent)
        {
            if (!LineTypeId.IsNull)
            {
                ent.LinetypeId = LineTypeId;                
            }
            else if (!string.IsNullOrEmpty(LineType))
            {
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
            else if (Color != null)
            {
                ent.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color);                
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
