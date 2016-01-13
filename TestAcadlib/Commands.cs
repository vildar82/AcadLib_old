using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using AcadLib.Blocks;
using Autodesk.AutoCAD.Geometry;
using AcadLib.Extensions;


[assembly: CommandClass(typeof (TestAcadlib.Commands))]

namespace TestAcadlib
{
    public class Commands
    {
      [CommandMethod("Test")]
      public void Test()
      {
         Document doc = AcAp.DocumentManager.MdiActiveDocument;
         Database db = doc.Database;
         Editor ed = doc.Editor;

         using (var t = db.TransactionManager.StartTransaction())
         {
            AlignedDimension dim = new AlignedDimension();
            dim.SetDatabaseDefaults(db);
            dim.XLine1Point = new Point3d(0, 0, 0);
            dim.XLine2Point = new Point3d(1000, 0, 0);
            dim.DimLinePoint = new Point3d(0, 100, 0);            

            var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForWrite) as BlockTableRecord;
            ms.AppendEntity(dim);
            t.AddNewlyCreatedDBObject(dim, true);

            dim.SetAnnotativeScale(25);            
            
            t.Commit();
         }
      }
   }
}
