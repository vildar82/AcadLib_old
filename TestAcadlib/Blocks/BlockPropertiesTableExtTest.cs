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
                var blRef = blRefId.GetObjectT<BlockReference>();
                var btr = blRef.BlockTableRecord.GetObjectT<BlockTableRecord>();

                var extDic = btr.ExtensionDictionary.GetObjectT<DBDictionary>();
                var graph = extDic.GetAt("ACAD_ENHANCEDBLOCK").GetObjectT<EvalGraph>();

                var nodeIds = graph.GetAllNodes();
                foreach (var i in nodeIds)
                {
                    var nodeId = (uint)i;
                    var node = ((dynamic)graph).GetNode(nodeId, OpenMode.ForRead, t);
                    if (node is BlockPropertiesTable)
                    {
                    }
                }

                t.Commit();
            }
        }
    }
}
