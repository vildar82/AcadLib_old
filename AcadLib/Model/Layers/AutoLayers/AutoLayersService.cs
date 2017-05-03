using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.AutoCAD.Runtime;

namespace AcadLib.Layers.AutoLayers
{
    public static class AutoLayersService
    {        
        static Document doc;
        static List<AutoLayer> autoLayers;
        static AutoLayer curAutoLayer;
        static List<ObjectId> idAddedEnts;        
        public static bool IsStarted { get; private set; }
        
        public static void Start()
        {
            // Определение приставки имени группы в слоях            
            if (string.IsNullOrEmpty(LayerExt.GroupLayerPrefix))
            {
                // Если пустая приставка, то не делать автослои.
                return;
            }
            Application.DocumentManager.DocumentActivated -= DocumentManager_DocumentActivated;
            Application.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;            
            autoLayers = GetAutoLayers();
            SubscribeDocument(Application.DocumentManager.MdiActiveDocument);
            IsStarted = true;
        }

        public static void Stop()
        {
            Application.DocumentManager.DocumentActivated -= DocumentManager_DocumentActivated;
            if (doc!= null)
            {
                doc.CommandWillStart -= Doc_CommandWillStart;                
                doc.CommandEnded -= Doc_CommandEnded;                
            }
            doc = null;
            curAutoLayer = null;
            autoLayers = null;
            idAddedEnts = null;
            IsStarted = false;
        }

        static void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            SubscribeDocument(e.Document);
        }

        private static void SubscribeDocument(Document document)
        {
            // Отписка в старом документе
            if (doc != null)
            {
                doc.CommandWillStart -= Doc_CommandWillStart;
                doc.CommandEnded -= Doc_CommandEnded;
                doc.CommandCancelled -= Doc_CommandCancelled;
            }
            doc = document;
            // Подписка на события команды нового документа
            doc.CommandWillStart -= Doc_CommandWillStart;
            doc.CommandWillStart += Doc_CommandWillStart;            
        }

        static void Doc_CommandWillStart(object sender, CommandEventArgs e)
        {
            // Для команд автослоев - подписка на добавление объектов в чертеж
            curAutoLayer = GetAutoLayerCommand(e.GlobalCommandName);
            if (curAutoLayer!= null)
            {                
                if (sender is Document doc)
                {
                    idAddedEnts = new List<ObjectId>();
                    doc.Database.ObjectAppended -= Database_ObjectAppended;
                    doc.Database.ObjectAppended += Database_ObjectAppended;
                    doc.CommandEnded -= Doc_CommandEnded;
                    doc.CommandEnded += Doc_CommandEnded;
                    doc.CommandCancelled -= Doc_CommandCancelled;
                    doc.CommandCancelled += Doc_CommandCancelled;
                }
            }
        }        

        static void Doc_CommandEnded(object sender, CommandEventArgs e)
        {
            EndCommand(sender as Document, e.GlobalCommandName);
        }
        private static void Doc_CommandCancelled(object sender, CommandEventArgs e)
        {
            EndCommand(sender as Document, e.GlobalCommandName);            
        }

        private static void EndCommand(Document document, string globalCommandName)
        {
            if (curAutoLayer != null && document != null)
            {
                document.Database.ObjectAppended -= Database_ObjectAppended;
                // Обработка объектов
                ProcessingAutoLayers(curAutoLayer, idAddedEnts);
                curAutoLayer = null;
                document.CommandEnded -= Doc_CommandEnded;
            }
        }

        static void Database_ObjectAppended(object sender, ObjectEventArgs e)
        {
            // Сбор всех id добавляемых объектов            
            if (curAutoLayer != null && idAddedEnts != null)
            {
                idAddedEnts.Add(e.DBObject.Id);
            }
        }        

        static List<AutoLayer> GetAutoLayers()
        {
            var autoLayers = new List<AutoLayer> {
                new AutoLayerDim()
            };
            return autoLayers;
        }

        static AutoLayer GetAutoLayerCommand(string globalCommandName)
        {
            return autoLayers.Find(f=>f.IsAutoLayerCommand(globalCommandName));
        }

        static void ProcessingAutoLayers(AutoLayer curAutoLayer, List<ObjectId> idAddedEnts)
        {
            var autoLayerEnts = curAutoLayer.GetAutoLayerEnts(idAddedEnts);
            using (var t = doc.TransactionManager.StartTransaction())
            {
                AutoLayerEntities(curAutoLayer, autoLayerEnts);
                t.Commit();
            }
        }

        private static void AutoLayerEntities(AutoLayer autoLayer, List<ObjectId> autoLayerEnts)
        {
            var layId = autoLayer.Layer.CheckLayerState();
            foreach (var idEnt in autoLayerEnts)
            {
                var ent = idEnt.GetObject<Entity>(OpenMode.ForWrite);
                if (ent != null && ent.LayerId != layId)
                {
                    ent.LayerId = layId;
                }
            }
        }

        /// <summary>
        /// Автослои для всех объектов чертежа
        /// </summary>
        public static void AutoLayersAll()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;            
            using (var t = doc.TransactionManager.StartTransaction())
            {
                var db = doc.Database;
                foreach (var idBtr in db.BlockTableId.GetObject<BlockTable>())
                {
                    var btr = idBtr.GetObject<BlockTableRecord>();
                    AutoLayersBtr(btr);
                }
                t.Commit();
            }
        }

        private static void AutoLayersBtr(BlockTableRecord btr)
        {
            var idEnts = btr.Cast<ObjectId>().ToList();
            var idBlRefs = idEnts.Where(w=>w.ObjectClass == RXObject.GetClass(typeof(BlockReference)));
            idEnts = idEnts.Except(idBlRefs).ToList();
            foreach (var idBlRef in idBlRefs)
            {
                var blRef = idBlRef.GetObject<BlockReference>();                
                AutoLayersBtr(blRef.BlockTableRecord.GetObject<BlockTableRecord>());
            }
            foreach (var autoLayer in autoLayers)
            {
                var autoLayerEnts = autoLayer.GetAutoLayerEnts(idEnts);
                AutoLayerEntities(autoLayer, autoLayerEnts);
                idEnts = idEnts.Except(autoLayerEnts).ToList();
            }
        }        

        public static string GetInfo()
        {            
            var info = string.Empty;
            if (string.IsNullOrEmpty(LayerExt.GroupLayerPrefix))
            {
                info = $"\nТекущая группа пользователя '{AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup}' не поддерживает автослои.";
            }
            else
            {
                info += IsStarted ? "Автослои включены" : "Автослои выключены";
                info += Environment.NewLine;
                foreach (var autoLayer in autoLayers)
                {
                    info += autoLayer.GetInfo() + Environment.NewLine;
                }
            }
            return info;
        }        
    }
}