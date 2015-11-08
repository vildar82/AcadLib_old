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
      public static void Insert(string blName)
      {
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
            Jigs.BlockInsertJig entJig = new Jigs.BlockInsertJig(br);

            // jig
            var pr = ed.Drag(entJig);
            if (pr.Status == PromptStatus.OK)
            {
               var btrBl = t.GetObject(idBlBtr, OpenMode.ForRead) as BlockTableRecord;
               var blRef = (BlockReference)entJig.GetEntity();
               var spaceBtr = (BlockTableRecord)t.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
               spaceBtr.AppendEntity(blRef);
               t.AddNewlyCreatedDBObject(blRef, true);
               if (btrBl.HasAttributeDefinitions)
                  AddAttributes(blRef, btrBl, t);
            }
            t.Commit();
         }
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