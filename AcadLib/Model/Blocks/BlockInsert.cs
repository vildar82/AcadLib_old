using System;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadLib.Blocks
{
    public static class BlockInsert
    {
        // Файл шаблонов блоков
        static string fileBlocks = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Blocks\Блоки-оформления.dwg");

        /// <summary>
        /// Вставка общего блока из файла Блоки-Оформления.
        /// Визуальная вставка с помошью Jig
        /// </summary>        
        public static ObjectId InsertCommonBlock(string blName, Database db)
        {
            // Выбор и вставка блока                 
            Block.CopyBlockFromExternalDrawing(blName, fileBlocks, db, DuplicateRecordCloning.Ignore);
            return Insert(blName);
        }

        public static ObjectId Insert(string blName, Layers.LayerInfo layer)
        {
            ObjectId idBlRefInsert = ObjectId.Null;
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (var t = db.TransactionManager.StartTransaction())
            {
                var bt = t.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (!bt.Has(blName))
                {
                    throw new Exception("Блок не определен в чертеже " + blName);
                }                             

                ObjectId idBlBtr = bt[blName];
                Point3d pt = Point3d.Origin;
                BlockReference br = new BlockReference(pt, idBlBtr);
                br.SetDatabaseDefaults();
                if (layer != null)
                {
                    Layers.LayerExt.CheckLayerState(layer);                    
                    br.Layer= layer.Name;
                }

                // jig
                Jigs.BlockInsertJig entJig = new Jigs.BlockInsertJig(br);
                var pr = ed.Drag(entJig);
                if (pr.Status == PromptStatus.OK)
                {
                    var btrBl = t.GetObject(idBlBtr, OpenMode.ForRead) as BlockTableRecord;
                    var blRef = (BlockReference)entJig.GetEntity();
                    var spaceBtr = (BlockTableRecord)t.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                    idBlRefInsert = spaceBtr.AppendEntity(blRef);
                    t.AddNewlyCreatedDBObject(blRef, true);
                    if (btrBl.HasAttributeDefinitions)
                        AddAttributes(blRef, btrBl, t);
                }
                t.Commit();
            }
            return idBlRefInsert;
        }

        public static ObjectId Insert(string blName, string layer)
        {
            Layers.LayerInfo layerInfo = new Layers.LayerInfo(layer);            
            return Insert(blName, layerInfo);
        }

        public static ObjectId Insert(string blName)
        {
            Layers.LayerInfo layer = null;
            return Insert(blName, layer);
        }

        public static void AddAttributes(BlockReference blRef, BlockTableRecord btrBl, Transaction t)
        {
            foreach (ObjectId idEnt in btrBl)
            {
                if (idEnt.ObjectClass.Name == "AcDbAttributeDefinition")
                {
                    var atrDef = t.GetObject(idEnt, OpenMode.ForRead) as AttributeDefinition;
                    if (!atrDef.Constant)
                    {
                        using (var atrRef = new AttributeReference())
                        {
                            atrRef.SetAttributeFromBlock(atrDef, blRef.BlockTransform);
                            atrRef.TextString = atrDef.TextString;
                            blRef.AttributeCollection.AppendAttribute(atrRef);
                            t.AddNewlyCreatedDBObject(atrRef, true);
                        }
                    }
                }
            }
        }
    }
}