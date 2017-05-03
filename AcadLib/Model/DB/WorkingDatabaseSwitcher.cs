using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib
{
   /// <summary>
   /// This class switches the WorkingDatabase. It was created for using in the 
   /// tests.
   /// </summary>
   public sealed class WorkingDatabaseSwitcher : IDisposable
   {
      Database oldDb = null;
      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="db">Target database.</param>
      public WorkingDatabaseSwitcher(Database db)
      {
         oldDb = HostApplicationServices.WorkingDatabase;
         HostApplicationServices. WorkingDatabase = db;
      }

      public void Dispose()
      {
         HostApplicationServices.WorkingDatabase = oldDb;
      }
   }
}
