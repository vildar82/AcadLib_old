using System;
using AcadLib.Layers;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    class DrawParameters : IDisposable
    {
        public LayerInfo Layer { get; set; }
        public Color Color { get; set; }
        public LineWeight? LineWeight { get; set; }
        public string LineType { get; set; }
        public double? LineTypeScale { get; set; }

        ObjectId oldLayer;
        Color oldColor;
        LineWeight oldLineWeight;
        ObjectId oldLineType;
        double oldLineScale;
        Database db;        

        public DrawParameters(Database db, LayerInfo layer = null, Color color = null, 
                            LineWeight? lineWeight = null, string lineType = null, double? lineTypeScale = null)
        {
            this.db = db;
            // Сохранение текущих свойств чертежа
            oldLayer = db.Clayer;            
            oldColor = db.Cecolor;
            oldLineWeight = db.Celweight;
            oldLineType = db.Celtype;
            oldLineScale = db.Celtscale;

            Layer = layer;
            Color = color;
            LineWeight = lineWeight;
            LineType = lineType;
            LineTypeScale = lineTypeScale;
            // установка новых свойств чертежу
            Setup();
        }

        /// <summary>
        /// Установка свойств в базу чертежа
        /// </summary>
        private void Setup()
        {
            if (Layer != null)
            {                
                db.Clayer = Layer.CheckLayerState();
            }
            // Цвет
            if (Color != null)
                db.Cecolor = Color;
            // Вес линии
            if (LineWeight != null)
                db.Celweight = LineWeight.Value;
            // Тип линии
            if (LineType != null)
                db.Celtype = db.LoadLineTypePIK(LineType);
            // Вес линии
            if (LineTypeScale != null)
                db.Celtscale = LineTypeScale.Value;
        }

        public void Dispose()
        {
            //Восстановление свойств
            // Слой
            if (Layer != null)                            
                db.Clayer = oldLayer;            
            // Цвет
            if (Color != null)
                db.Cecolor = oldColor;
            // Вес линии
            if (LineWeight != null)
                db.Celweight = oldLineWeight;
            // Тип линии
            if (LineType != null)
                db.Celtype = oldLineType;
            // Вес линии
            if (LineTypeScale != null)
                db.Celtscale = oldLineScale;
        }
    }
}
