using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AcadLib.Layers;
using Autodesk.AutoCAD.DatabaseServices;
using MicroMvvm;
using Visibility = System.Windows.Visibility;

namespace AcadLib.Errors.UI
{
    public class ErrorModelOne : ErrorModelBase
    {
        public ErrorModelList Parent { get; set; }

        public ErrorModelOne(IError err, ErrorModelList parent) : base(err)
        {
            VisibilityCount = Visibility.Collapsed;
            Parent = parent;
            if (parent == null)
            {
                Message = err.Message;
                MarginHeader = new Thickness(32, 2, 2, 2);
            }
            else
            {
                Message = err.Message.Length > err.Group.Length
                    ? err.Message.Substring(err.Group.Length)
                    : err.Message;
            }
            
            AddButtons = err.AddButtons;
            // Добавить кнопку, для отрисовки визуализации на чертежа
            if (HasVisuals)
            {
                if (AddButtons == null) AddButtons = new List<ErrorAddButton>();
                var visCommand = new RelayCommand(AddVisualsToDrawing, () => Error?.Visuals?.Any() == true);
                var visButton = new ErrorAddButton
                {
                    Name = "Отрисовка",
                    Tooltip = "Добавить визуализацию ошибки в чертеж.",
                    Click = visCommand
                };
                AddButtons.Add(visButton);
            }
        }

        private void AddVisualsToDrawing()
        {
            try
            {
                var doc = AcadHelper.Doc;
                var db = doc.Database;
                using (doc.LockDocument())
                using (var t = db.TransactionManager.StartTransaction())
                {
                    var layerVisual = LayerExt.CheckLayerState("visuals");
                    var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject<BlockTableRecord>(OpenMode.ForWrite);
                    var fEnt = Error.Visuals.First();
                    var fEntExt = new Extents3d();
                    var fEntId = ObjectId.Null;
                    foreach (var entity in Error.Visuals)
                    {
                        var entClone = (Entity)entity.Clone();
                        entClone.LayerId = layerVisual;
                        ms.AppendEntity(entClone);
                        t.AddNewlyCreatedDBObject(entClone, true);
                        if (fEnt == entity)
                        {
                            fEntId = entClone.Id;
                            try
                            {
                                fEntExt = entClone.GeometricExtents;
                            }
                            catch
                            {
                                //
                            }
                        }
                        entity.Dispose();
                    }
                    if (!Error.HasEntity && !fEntId.IsNull)
                    {
                        Error.HasEntity = true;
                        Error.IdEnt = fEntId;
                        Error.Extents = fEntExt;
                        HasShow = true;
                    }
                    Error.Visuals = new List<Entity>();
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
