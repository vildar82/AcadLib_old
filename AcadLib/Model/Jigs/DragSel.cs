using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Jigs
{
    /// <summary>
    /// Перемещение (drag) группы объектов за мышкой
    /// </summary>
    public static class DragSel
    {
        /// <summary>
        /// Перемещение объектов
        /// Открытая транзакция не требуется
        /// При отмене пользователем - объекты удаляются
        /// </summary>
        /// <param name="ed"></param>
        /// <param name="ids"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static bool Drag(this Editor ed, ObjectId[] ids, Point3d pt)
        {
            if (ids == null || !ids.Any()) return false;
            SelectionSet selSet = SelectionSet.FromObjectIds(ids);
            PromptPointResult ppr = ed.Drag(selSet, "\nТочка вставки:", (Point3d ptInput, ref Matrix3d mat) =>
            {
                if (ptInput.IsEqualTo(pt)) return SamplerStatus.NoChange;
                mat = Matrix3d.Displacement(pt.GetVectorTo(ptInput));
                //pt = ptInput;
                return SamplerStatus.OK;
            });

            if (ppr.Status == PromptStatus.OK)
            {
                using (var t = ed.Document.TransactionManager.StartTransaction())
                {
                    foreach (var item in ids)
                    {
                        var mat = Matrix3d.Displacement(pt.GetVectorTo(ppr.Value));
                        var ent = item.GetObject(OpenMode.ForWrite, false, true) as Entity;
                        ent.TransformBy(mat);
                    }
                    t.Commit();
                }

                return true;
            }
            else
            {
                using (var t = ed.Document.TransactionManager.StartTransaction())
                {
                    foreach (var id in ids)
                    {
                        var ent = id.GetObject(OpenMode.ForWrite);
                        ent.Erase();
                    }
                    t.Commit();
                }
                return false;
            }
        }

        /// <summary>
        /// Вставка в 0 объектов в текущее пространство и перемещение в указанную пользователем точку (Drag)
        /// </summary>        
        /// <param name="ents">Объекты не резиденты чертежа (будут вставленны в текущее пространство листа)</param>
        /// <param name="height">Высота разбиения объектов по высоте (вставка в столбик, пока высота столбца меньше заданной высоты)</param>        
        public static void Drag(this Editor ed, List<Entity> ents, double height = 10000)
        {
            var db = ed.Document.Database;
            var ids = new List<ObjectId>();
            using (var t = db.TransactionManager.StartTransaction())
            {
                var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;

                foreach (var ent in ents)
                {
                    cs.AppendEntity(ent);
                    t.AddNewlyCreatedDBObject(ent, true);
                    ids.Add(ent.Id);
                }
                t.Commit();
            }
            using (var t = db.TransactionManager.StartTransaction())
            {
                var pt = Point3d.Origin;
                double sumH = 0;
                double maxX = 0;

                foreach (var entId in ids)
                {
                    var ent = entId.GetObject(OpenMode.ForWrite, false, true) as Entity;
                    var entExt = ent.GeometricExtents;
                    // левая верхняя точка объекта
                    var ptEntLT = new Point3d(entExt.MinPoint.X, entExt.MaxPoint.Y, 0);
                    // вектор от мин точки границы до лев верхней
                    var vecFromMinPtToLT = ptEntLT - entExt.MinPoint;
                    var vecHWithDelta = vecFromMinPtToLT * 1.1;

                    // перемещение объекта в точку вствки (левый верхний угол границы)
                    var move = Matrix3d.Displacement(pt - ptEntLT);
                    ent.TransformBy(move);

                    var curX = pt.X + (entExt.MaxPoint.X - entExt.MinPoint.X)*1.1;
                    if (curX > maxX)
                        maxX = curX;

                    sumH += vecHWithDelta.Length;
                    if (sumH>= height)
                    {
                        sumH = 0;
                        pt = new Point3d(maxX, 0, 0);
                    }
                    else
                    {
                        pt = pt - vecHWithDelta;
                    }                    
                }
                t.Commit();
            }           
            if (ids.Any())
            {
                ed.Drag(ids.ToArray(), Point3d.Origin);
            }
        }
    }
}
