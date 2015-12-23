using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace AcadLib.Errors
{
   public static class Inspector
   {
      private static Database _db;
      private static Document _doc;
      private static Editor _ed;      
      private static List<Error> _errors;

      public static List<Error> Errors { get { return _errors; } }

      public static bool HasErrors { get { return _errors.Count > 0; } }      

      static Inspector()
      {         
         Clear();
      }

      public static void Clear ()
      {
         _doc = Application.DocumentManager.MdiActiveDocument;
         _db = _doc.Database;
         _ed = _doc.Editor;
         _errors = new List<Error>();         
      }

      public static void AddError (string msg)
      {
         var err = new Error(msg);
         _errors.Add(err);
      }
      public static void AddError(string msg, params object[] args)
      {
         var err = new Error(string.Format(msg, args));
         _errors.Add(err);
      }

      public static void AddError(string msg, Entity ent)
      {
         var err = new Error(msg, ent);
         _errors.Add(err);
      }
      public static void AddError(string msg, Entity ent, Extents3d ext)
      {
         var err = new Error(msg, ext, ent);
         _errors.Add(err);
      }
      public static void AddError(string msg, Extents3d ext, ObjectId idEnt)
      {
         var err = new Error(msg, ext, idEnt);
         _errors.Add(err);
      }
      public static void AddError(string msg, ObjectId idEnt)
      {
         var err = new Error(msg, idEnt);
         _errors.Add(err);
      }

      public static void Show()
      {
         Application.ShowModelessDialog(new FormError());
      }
   }
}