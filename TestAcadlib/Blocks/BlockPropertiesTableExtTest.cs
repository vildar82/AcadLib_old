using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace TestAcadlib.Blocks
{
    public class BlockPropertiesTableExtTest
    {
        [CommandMethod(nameof(TestBlockPropertiesTableExtCommand))]
        public void TestBlockPropertiesTableExtCommand()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var blRefId = ed.SelectEntity<BlockReference>("Выбери дин блок", "Выбери дин блок");
            using (var t = db.TransactionManager.StartTransaction())
            {
                var blRef = blRefId.GetObject<BlockReference>();
                var btr = blRef.BlockTableRecord.GetObject<BlockTableRecord>();

                BlockPropertiesTable bpt = null;
                //var bpt = btr.GetBlockPropertiesTable();

                var extDic = btr.ExtensionDictionary.GetObject<DBDictionary>();
                var graph = extDic.GetAt("ACAD_ENHANCEDBLOCK").GetObject<EvalGraph>();
                //var bpt = graph.GetAllNodes()
                //    .Select(f => graph.GetNode((uint)f, OpenMode.ForRead, t) as BlockPropertiesTable)
                //    .FirstOrDefault(w => w != null);


                var nodeIds = graph.GetAllNodes();
                foreach (uint nodeId in nodeIds)
                {
                    var node = ((dynamic)graph).GetNode(nodeId, OpenMode.ForRead, t);
                    if (node is BlockPropertiesTable)
                    {
                        bpt = (BlockPropertiesTable)node;
                    }
                }
                //.Select(f => graph.GetNode((uint)f, OpenMode.ForRead, t) as BlockPropertiesTable)
                //.FirstOrDefault(w => w != null);

                t.Commit();
            }
        }
    }
}
