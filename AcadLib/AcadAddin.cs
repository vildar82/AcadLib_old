using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

[assembly:ExtensionApplication(typeof(AcadLib.AcadAddin))]
[assembly:CommandClass(typeof(AcadLib.AcadAddin))]

namespace AcadLib
{
   public class AcadAddin : IExtensionApplication
   {
      private static string about = "\nБиблиотека AcadLib версия: " + Assembly.GetExecutingAssembly().GetName().Version;                 

      [CommandMethod("PIK", "About-AcadLib", CommandFlags.Modal)]
      public void AboutCommand ()
      {
         aboutMsg();
      }     

      public void Initialize()
      {
         aboutMsg();
      }

      public void Terminate()
      {         
      }

      private static void aboutMsg()
      {
         Document doc = Application.DocumentManager.MdiActiveDocument;
         if (doc == null) return;
         doc.Editor.WriteMessage(about);
      }
   }
}
