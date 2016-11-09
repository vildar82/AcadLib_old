using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Blocks
{
    /// <summary>
    /// Базовое описание блока
    /// </summary>
    public class  BlockBase : IBlock
    {
        private bool _alreadyCalcExtents;
        private Extents3d _extentsToShow;
        private bool _isNullExtents;

        public Database Db { get; private set; }
        /// <summary>
        /// Имя блока - эффективное
        /// </summary>
        public string BlName { get; set; }
        public string BlLayer { get; set; }
        public virtual Color Color { get; set; }
        public Point3d Position { get; set; }
        /// <summary>
        /// Границы блока Bounds
        /// </summary>
        public virtual Extents3d? Bounds { get; set; }
        /// <summary>
        /// Id вхождения блока
        /// </summary>
        public ObjectId IdBlRef { get; set; }
        /// <summary>
        /// Id определения блока - BklockTableRecord (для анонимных - анонимное).
        /// </summary>
        public ObjectId IdBtr { get; set; }
        /// <summary>
        /// Пространство в который вставлен этот блок (определение блока)
        /// </summary>
        public ObjectId IdBtrOwner { get; set; }
        /// <summary>
        /// Параметры - атрибутв и динамические
        /// </summary>
        public List<Property> Properties { get; set; }
        public Error Error { get; set; }
        public Matrix3d Transform { get; set; }

        /// <summary>
        /// Блок - по имени и ссылке на вхождение блока
        /// Заполняются параметры блока. и граница Bounds
        /// </summary>        
        public BlockBase (BlockReference blRef, string blName)
        {
            if (blRef == null)                return;                           
            BlName = blName;
            Update(blRef);
        }

        private Color GetColor (BlockReference blRef)
        {
            if (blRef.Color.IsByLayer)
            {
                using (var lay = blRef.LayerId.Open(OpenMode.ForRead) as LayerTableRecord)
                {
                    return lay.Color;
                }
            }
            else
            {
                return blRef.Color;
            }
        }

        /// <summary>
        /// Границы для показа пользователю
        /// </summary>
        public Extents3d ExtentsToShow {
            get {
                if (!_alreadyCalcExtents)
                {                    
#pragma warning disable CS0618
                    using (var blRef = IdBlRef.Open(OpenMode.ForRead, false, true) as BlockReference)
                    {
                        try
                        {
                            _extentsToShow = blRef.GeometricExtents;
                            _alreadyCalcExtents = true;
                        }
                        catch
                        {
                            _isNullExtents = true;
                            _extentsToShow = new Extents3d(new Point3d(blRef.Position.X - 100, blRef.Position.Y - 100, 0),
                                new Point3d(blRef.Position.X + 100, blRef.Position.Y + 100, 0));
                        }
                    }
#pragma warning restore CS0618
                }                
                return _extentsToShow;
            }
            set {
                _alreadyCalcExtents = true;
                _extentsToShow = value;
            }
        }

        /// <summary>
        /// Показ блока (по границе) пользователю с миганием
        /// С проверкой чертежа и блокировкой.
        /// </summary>
        public virtual void Show ()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc != null)
            {
                if (doc.Database != IdBlRef.Database)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog($"Переключитесь на чертеж {Path.GetFileNameWithoutExtension(IdBlRef.Database.Filename)}");
                    return;
                }
                using (doc.LockDocument())
                {
                    Editor ed = doc.Editor;
                    var ext = ExtentsToShow;
                    if (_isNullExtents)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Границы объекта не определены.");
                    }
                    ed.Zoom(ext);
                    IdBlRef.FlickObjectHighlight(2, 100, 100);
                }
            }
        }

        public void Delete()
        {
            var blRef = IdBlRef.GetObject(OpenMode.ForWrite);
            blRef.Erase();
        }

        /// <summary>
        /// преобразование object value свойства Property в указанный тип
        /// Тип T должен точно соответствовать типу object value Property
        /// </summary>        
        /// <param name="propMatch">Имя параметрв или соответствие</param>
        /// <param name="isRequired">Добавлять ли ошибку если нет этого параметра.</param>
        /// <param name="exactMatch">Точное соответствие имени параметра, или заданному паттерну.</param>
        public T GetPropValue<T> (string propMatch, bool isRequired = true, bool exactMatch = true)
        {            
            T resVal = default(T);
            if (exactMatch)
            {
                propMatch = $"^{propMatch}$";
            }            
            Property prop = GetProperty(propMatch, isRequired);
            if (prop != null)
            {
                try
                {
                    var value = prop.Value;
                    var culture = CultureInfo.InvariantCulture;
                    if (value is string && typeof(T) == typeof(double))
                    {
                        value = ((string)value).Replace(",", ".");
                        culture = new CultureInfo("en-US");
                    }
                    resVal = (T)Convert.ChangeType(value, typeof(T), culture);
                }
                catch
                {
                    AddError($"Недопустимый тип значения параметра '{propMatch}'= {prop.Value}.");
                }
            }
            return resVal;
        }        

        protected Property GetProperty (string nameMatch, bool isRequired = true)
        {
            Property prop = Properties.Find(p=> Regex.IsMatch(p.Name, nameMatch, RegexOptions.IgnoreCase));
            if (prop == null && isRequired)
            {
                AddError($"Не определен параметр '{nameMatch}'.");
            }
            return prop;            
        }

        public void FillPropValue (string propMatch, object value, bool exactMatch = true, bool isRequired = true)
        {
            if (exactMatch)
            {
                propMatch = $"^{propMatch}$";
            }
            FillProp(GetProperty(propMatch, isRequired), value);
        }

        protected void FillProp (Property prop, object value)
        {
            if (prop == null) return;
            if (prop.Type == PropertyType.Attribute && !prop.IdAtrRef.IsNull)
            {
                var atr = prop.IdAtrRef.GetObject(OpenMode.ForWrite, false, true) as AttributeReference;
                atr.TextString = value?.ToString() ?? "";
            }
            else if (prop.Type == PropertyType.Dynamic)
            {
                if (value == null) return;
                var blRef = IdBlRef.GetObject(OpenMode.ForWrite) as BlockReference;
                var dynProp = blRef.DynamicBlockReferencePropertyCollection.Cast<DynamicBlockReferenceProperty>()
                    .FirstOrDefault(p => p.PropertyName.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                if (dynProp != null)
                {
                    try
                    {
                        dynProp.Value = value;
                    }
                    catch
                    {
                        Inspector.AddError($"Не удалосось установить динамический параметр {prop.Name} со значением {prop.Value} в блок {BlName}",
                            IdBlRef, System.Drawing.SystemIcons.Error);
                    }
                }
            }
        }

        public void AddError (string msg)
        {
            if (Error == null)
            {
                Error = new Error($"Ошибка в блоке '{BlName}' {IdBlRef}: ", IdBlRef, System.Drawing.SystemIcons.Error);
                Inspector.AddError(Error);
            }
            Error.AdditionToMessage(msg);
        }

        /// <summary>
        /// Поиск полилинии в этом блоке на слое
        /// </summary>        
        public List<Polyline> FindPolylineInLayer (string layer)
        {
            var btr = this.IdBtr.GetObject(OpenMode.ForRead) as BlockTableRecord;
            var allPls = btr.GetObjects<Polyline>(OpenMode.ForRead);
            var pls = allPls.Where(p =>p.Visible && p.Layer.Equals(layer, StringComparison.OrdinalIgnoreCase)).ToList();
            return pls;
        }

        /// <summary>
        /// Копирование объекта из этого блока в модель (btr)
        /// </summary>
        /// <param name="idBtrNew">Куда копировать</param>        
        /// <param name="idEnt">Что копировать</param>
        /// <returns>Скопированный объект</returns>
        public ObjectId CopyEntToModel (ObjectId idBtrNew, ObjectId idEnt)
        {
            if (!idEnt.IsNull)
            {
                var idCopy = idEnt.CopyEnt(idBtrNew);
                using (var entCopy = idCopy.GetObject(OpenMode.ForWrite, false, true) as Entity)
                {
                    entCopy.TransformBy(Transform);
                    return entCopy.Id;
                }
            }
            return ObjectId.Null;
        }

        public bool Equals (IBlock other)
        {
            // Если все параметры совпадают
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            var res = new HashSet<Property>(Properties).SetEquals(other.Properties);
            return res;
        }

        public override int GetHashCode ()
        {
            return BlName.GetHashCode();
        }

        public void Update (BlockReference blRef)
        {
            // Считать блок заново
            if (blRef == null)
            {
                return;
            }
            else
            {
                Db = blRef.Database;
                IdBtrOwner = blRef.OwnerId;
                IdBlRef = blRef.Id;
                IdBtr = blRef.BlockTableRecord;
                //BlName = blName;
                BlLayer = blRef.Layer;
                Properties = Property.GetAllProperties(blRef);
                Bounds = blRef.Bounds;
                Position = blRef.Position;
                Transform = blRef.BlockTransform;
                Color = GetColor(blRef);
            }
        }
    }
}