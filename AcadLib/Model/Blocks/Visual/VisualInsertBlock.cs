using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows.Data;

namespace AcadLib.Blocks.Visual
{
    public static class VisualInsertBlock
    {
        private static Dictionary<string, List<VisualBlock>> dictFiles = new Dictionary<string, List<VisualBlock>>();        

        public static void InsertBlock(string fileBlocks, Predicate<string> filter, Layers.LayerInfo layer = null)
        {
            List<VisualBlock> visuals;
            if (!dictFiles.TryGetValue(fileBlocks, out visuals))
            {
                visuals = LoadVisuals(fileBlocks, filter);
                dictFiles.Add(fileBlocks, visuals);
            }

            WindowVisualBlocks winVisual = new WindowVisualBlocks(visuals);
            var dlgRes = Application.ShowModalWindow(winVisual);
            if (dlgRes.HasValue && dlgRes.Value)
            {                
                insert(winVisual.Selected, fileBlocks, layer);
            }
        }        

        public static List<VisualBlock> LoadVisuals(string file, Predicate<string> filter)
        {
            List<VisualBlock> visualBlocks = new List<VisualBlock>();
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
                            VisualBlock visualBl = new VisualBlock(btr);
                            visualBlocks.Add(visualBl);
                        }
                    }
                    t.Commit();
                }
            }
            return visualBlocks;
        }

        private static void insert(VisualBlock selected, string fileBlocks, Layers.LayerInfo layer)
        {
            if (selected == null) return;
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var idBtr = getInsertBtr(selected.Name, fileBlocks, db);
            AcadLib.Blocks.BlockInsert.Insert(selected.Name, layer);
        }

        private static ObjectId getInsertBtr(string name, string fileBlocks, Database dbdest)
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
            return  AcadLib.Blocks.Block.CopyBlockFromExternalDrawing(name, fileBlocks, dbdest);
        }
    }
}
