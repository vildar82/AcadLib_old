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
      private string _name;
      private bool _isOff;
      private bool _isFrozen;
      private bool _isPlotable;
      private bool _isLocked;
      private Color _color;      
      private ObjectId _linetypeObjectId;
      private LineWeight _lineWeight;

      public string Name
      {
         get { return _name; }
         set { setName(value); }
      }

      public bool IsOff { get { return _isOff; } set { _isOff = value; } }
      public bool IsFrozen { get { return _isFrozen; } set { _isFrozen = value; } }
      public bool IsPlotable { get { return _isPlotable; } set { _isPlotable = value; } }
      public bool IsLocked { get { return _isLocked; } set { _isLocked = value; } }
      public Color Color { get { return _color; } set { _color = value; } }      
      public LineWeight LineWeight { get { return _lineWeight; } set { _lineWeight = value; } }
      public ObjectId LinetypeObjectId { get { return _linetypeObjectId; } set { _linetypeObjectId = value; } }

      public LayerInfo (string name)
      {
         setName(name);
         _color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, 7);
         _lineWeight = LineWeight.ByLineWeightDefault;
         _isPlotable = true;
      }

      private void setName(string name)
      {
         if (name.IsValidDbSymbolName())         
            _name = name;         
         else         
            throw new Exception(string.Format("Недопустимое имя слоя - {0}", name));         
      }      
   }
}
