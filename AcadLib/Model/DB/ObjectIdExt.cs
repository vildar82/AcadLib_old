using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
    public static class ObjectIdExt
    {
        /// <summary>
        /// Функция производит "мигание" объектом при помощи Highlight/Unhighlight
        /// </summary>
        /// <param name="id">ObjectId для примитива</param>
        /// <param name="num">Количество "миганий"</param>
        /// <param name="delay1">Длительность "подсвеченного" состояния</param>
        /// <param name="delay2">Длительность "неподсвеченного" состояния</param>
        static public void FlickObjectHighlight(this ObjectId id, int num, int delay1, int delay2)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            for (int i = 0; i < num; i++)
            {
                // Highlight entity
                using (DocumentLock doclock = doc.LockDocument())
                {
                    using (Transaction tr = doc.TransactionManager.StartTransaction())
                    {
                        Entity en = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                        ObjectId[] ids = new ObjectId[1]; ids[0] = id;
                        SubentityId index = new SubentityId(SubentityType.Null, 0);
                        FullSubentityPath path = new FullSubentityPath(ids, index);
                        en.Highlight(path, true);
                        tr.Commit();
                    }
                }
                doc.Editor.UpdateScreen();
                // Wait for delay1 msecs
                Thread.Sleep(delay1);
                // Unhighlight entity
                using (DocumentLock doclock = doc.LockDocument())
                {
                    using (Transaction tr = doc.TransactionManager.StartTransaction())
                    {
                        Entity en = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                        ObjectId[] ids = new ObjectId[1]; ids[0] = id;
                        SubentityId index = new SubentityId(SubentityType.Null, 0);
                        FullSubentityPath path = new FullSubentityPath(ids, index);
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
            Document doc = Application.DocumentManager.MdiActiveDocument;
            for (int i = 0; i < num; i++)
            {
                // Highlight entity
                using (DocumentLock doclock = doc.LockDocument())
                {
                    using (Transaction tr = doc.TransactionManager.StartTransaction())
                    {
                        SubentityId subId = new SubentityId(SubentityType.Null, 0);
                        FullSubentityPath path = new FullSubentityPath(idsPath, subId);
                        var ent = idsPath[0].GetObject(OpenMode.ForRead) as Entity;
                        ent.Highlight(path, true);
                        tr.Commit();
                    }
                }
                doc.Editor.UpdateScreen();
                // Wait for delay1 msecs
                Thread.Sleep(delay1);
                // Unhighlight entity
                using (DocumentLock doclock = doc.LockDocument())
                {
                    using (Transaction tr = doc.TransactionManager.StartTransaction())
                    {
                        SubentityId subId = new SubentityId(SubentityType.Null, 0);
                        FullSubentityPath path = new FullSubentityPath(idsPath, subId);
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
    }
}
