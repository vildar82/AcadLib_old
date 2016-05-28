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
        public static bool Drag(Editor ed, ObjectId[] ids, Point3d pt)
        {
            SelectionSet selSet = SelectionSet.FromObjectIds(ids);
            PromptPointResult ppr = ed.Drag(selSet, "\nТочка вставки:", (Point3d ptInput, ref Matrix3d mat) =>
            {
                if (ptInput == pt) return SamplerStatus.NoChange;
                mat = Matrix3d.Displacement(pt.GetVectorTo(ptInput));
                using (var t = ed.Document.TransactionManager.StartTransaction())
                {
                    foreach (var item in ids)
                    {
                        var ent = item.GetObject(OpenMode.ForWrite, false, true) as Entity;
                        ent.TransformBy(mat);
                    }
                    t.Commit();
                }
                
                pt = ptInput;
                return SamplerStatus.OK;
            });

            if (ppr.Status == PromptStatus.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
