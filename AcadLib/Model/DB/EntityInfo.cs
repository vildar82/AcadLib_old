﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.DB
{
    public class EntityInfo:IEquatable<EntityInfo>
    {
        public string Layer { get; set; }
        public string Linetype { get; set; }
        public LineWeight Lineweight { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public System.Drawing.Color Color { get; set; }
        public Extents3d Extents { get; set; }
        public ObjectId Id { get; set; }

        public EntityInfo(Entity ent)
        {
            ClassName = ent.GetRXClass().Name;
            Id = ent.Id;
            if (ent.Bounds.HasValue)
            {
                Extents = ent.Bounds.Value;
            }
            ClassId = ent.ClassID;
            Color = ent.Color.ColorValue;
            Layer = ent.Layer;
            Linetype = ent.Linetype;
            Lineweight = ent.LineWeight;
        }

        public bool Equals(EntityInfo other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            return Extents.Equals(other.Extents) &&
               ClassId.Equals(other.ClassId) &&
               Color.Equals(other.Color) &&
               Layer.Equals(other.Layer) &&
               Linetype.Equals(other.Linetype) &&
               Lineweight.Equals(other.Lineweight);
        }        
    }
}
