using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows.Data;
using AcadLib.Blocks.Visual.UI;

namespace AcadLib.Blocks.Visual
{
    public static class VisualInsertBlock
    {
        private static Dictionary<Predicate<string>, List<IVisualBlock>> dictFiles = new Dictionary<Predicate<string>, List<IVisualBlock>>();        

        public static void InsertBlock(string fileBlocks, Predicate<string> filter, Layers.LayerInfo layer = null)
        {
            List<IVisualBlock> visuals;
            if (!dictFiles.TryGetValue(filter, out visuals))
            {                
                visuals = LoadVisuals(fileBlocks, filter);
                dictFiles.Add(filter, visuals);
            }

            var vm = new VisualBlocksViewModel(visuals);
            var winVisual = new WindowVisualBlocks(vm);
            Application.ShowModalWindow(winVisual);            
        }        

        public static List<IVisualBlock> LoadVisuals(string file, Predicate<string> filter)
        {
            var visualBlocks = new List<IVisualBlock>();
            using (var dbTemp = new Database(false, true))
            {
                dbTemp.ReadDwgFile(file, FileOpenMode.OpenForReadAndReadShare, true, "");
                using (var t = dbTemp.TransactionManager.StartTransaction())
                {
                    var bt = dbTemp.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                    foreach (var idBtr in bt)
                    {
                        var btr = idBtr.GetObject(OpenMode.ForRead) as BlockTableRecord;
                        if (filter(btr.Name))
                        {
                            var visualBl = new VisualBlock(btr);
                            visualBl.File = file;
                            visualBlocks.Add(visualBl);
                        }
                    }
                    t.Commit();
                }
                var alpha = Comparers.AlphanumComparator.New;
                visualBlocks.Sort((v1, v2) => alpha.Compare(v1.Name, v2.Name));
            }
            return visualBlocks;
        }

        /// <summary>
        /// Переопределенеи блока
        /// </summary>        
        public static void Redefine(IVisualBlock block)
        {
            if (block == null) return;
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            using (doc.LockDocument())
            {
                Block.Redefine(block.Name, block.File, doc.Database);
            }
        }

        public static void Insert(IVisualBlock block, Layers.LayerInfo layer = null)
        {
            if (block == null) return;
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var idBtr = GetInsertBtr(block.Name, block.File, db);
            BlockInsert.Insert(block.Name, layer);
        }

        private static ObjectId GetInsertBtr(string name, string fileBlocks, Database dbdest)
        {
            // Есть ли уже блок в текущем файле
            using (var bt = dbdest.BlockTableId.Open( OpenMode.ForRead)as BlockTable)
            {
                if (bt.Has(name))
                {
                    return bt[name];
                }
            }
            // Копирование блока из файла шаблона
            return Block.CopyBlockFromExternalDrawing(name, fileBlocks, dbdest);
        }
    }
}
