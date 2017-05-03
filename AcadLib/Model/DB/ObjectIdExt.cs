using System;
using System.Threading;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib
{
    public static class ObjectIdExt
    {
        public static void ShowEnt(this ObjectId id, int num, int delay1, int delay2)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (doc == null || !id.IsValidEx()) return;

            using (doc.LockDocument())
            using (var t = id.Database.TransactionManager.StartTransaction())
            {
                var ent = id.GetObject(OpenMode.ForRead) as Entity;
                if (ent != null)
                {
                    try
                    {
                        doc.Editor.Zoom(ent.GeometricExtents);
                        id.FlickObjectHighlight(num, delay1, delay2);
                    }
                    catch { }
                }
                t.Commit();
            }
        }
        public static void ShowEnt (this ObjectId id)
        {
            ShowEnt(id, 2,100,100);
        }

        public static void ShowEnt(this ObjectId id, Extents3d ext, Document docOrig)
        {
            var curDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (docOrig != curDoc)
            {
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog($"Должен быть активен документ {docOrig.Name}");
            }
            else
            {
                if (ext.Diagonal() > 1)
                {                    
                    docOrig.Editor.Zoom(ext);                                        
                    id.FlickObjectHighlight(2, 100, 100);                    
                }
                else
                {
                    Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Границы элемента не определены");
                }
            }
        }

        /// <summary>
        /// Функция производит "мигание" объектом при помощи Highlight/Unhighlight
        /// </summary>
        /// <param name="id">ObjectId для примитива</param>
        /// <param name="num">Количество "миганий"</param>
        /// <param name="delay1">Длительность "подсвеченного" состояния</param>
        /// <param name="delay2">Длительность "неподсвеченного" состояния</param>
        static public void FlickObjectHighlight(this ObjectId id, int num, int delay1, int delay2)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            for (var i = 0; i < num; i++)
            {
                // Highlight entity
                using (var doclock = doc.LockDocument())
                using (var tr = doc.TransactionManager.StartTransaction())
                {
                    var en = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    var ids = new ObjectId[1]; ids[0] = id;
                    var index = new SubentityId(SubentityType.Null, IntPtr.Zero);
                    var path = new FullSubentityPath(ids, index);
                    en.Highlight(path, true);
                    tr.Commit();
                }                
                doc.Editor.UpdateScreen();
                // Wait for delay1 msecs
                Thread.Sleep(delay1);
                // Unhighlight entity
                using (var doclock = doc.LockDocument())
                {
                    using (var tr = doc.TransactionManager.StartTransaction())
                    {
                        var en = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                        var ids = new ObjectId[1]; ids[0] = id;
                        var index = new SubentityId(SubentityType.Null, IntPtr.Zero);
                        var path = new FullSubentityPath(ids, index);
                        en.Unhighlight(path, true);
                        tr.Commit();
                    }
                }
                doc.Editor.UpdateScreen();
                // Wait for delay2 msecs
                Thread.Sleep(delay2);
            }
        }

        /// <summary>
        /// Функция производит "мигание" подобъектом при помощи Highlight/Unhighlight
        /// </summary>
        /// <param name="idsPath">Цепочка вложенности объектов. BlockReference->Subentity</param>
        /// <param name="num">Количество "миганий"</param>
        /// <param name="delay1">Длительность "подсвеченного" состояния</param>
        /// <param name="delay2">Длительность "неподсвеченного" состояния</param>
        static public void FlickSubentityHighlight(ObjectId[] idsPath, int num, int delay1, int delay2)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            for (var i = 0; i < num; i++)
            {
                // Highlight entity
                using (var doclock = doc.LockDocument())
                {
                    using (var tr = doc.TransactionManager.StartTransaction())
                    {
                        var subId = new SubentityId(SubentityType.Null, IntPtr.Zero);
                        var path = new FullSubentityPath(idsPath, subId);
                        var ent = idsPath[0].GetObject(OpenMode.ForRead) as Entity;
                        ent.Highlight(path, true);
                        tr.Commit();
                    }
                }
                doc.Editor.UpdateScreen();
                // Wait for delay1 msecs
                Thread.Sleep(delay1);
                // Unhighlight entity
                using (var doclock = doc.LockDocument())
                {
                    using (var tr = doc.TransactionManager.StartTransaction())
                    {
                        var subId = new SubentityId(SubentityType.Null, IntPtr.Zero);
                        var path = new FullSubentityPath(idsPath, subId);
                        var ent = idsPath[0].GetObject(OpenMode.ForRead) as Entity;
                        ent.Unhighlight(path, true);
                        tr.Commit();
                    }
                }
                doc.Editor.UpdateScreen();
                // Wait for delay2 msecs
                Thread.Sleep(delay2);
            }
        }

        /// <summary>
        /// Копирование объекта в одной базе
        /// </summary>
        /// <param name="idEnt">Копируемый объект</param>
        /// <param name="idBtrOwner">Куда копировать (контейнер - BlockTableRecord)</param>                
        public static ObjectId CopyEnt (this ObjectId idEnt, ObjectId idBtrOwner)
        {
            var db = idEnt.Database;
            var map = new IdMapping();
            var ids = new ObjectIdCollection(new[] { idEnt });
            db.DeepCloneObjects(ids, idBtrOwner, map, false);
            return map[idEnt].Value;
        }       

        public static bool IsValidEx (this ObjectId id)
        {
            return id.IsValid && !id.IsNull && !id.IsErased;            
        }
    }
}
