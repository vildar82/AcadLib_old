using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(AcadLib.Commands))]

namespace AcadLib
{
   public class Commands
   {
      [CommandMethod("PIK", "PIK-Acadlib-About", CommandFlags.Modal)]
      public void About ()
      {
         Document doc = Application.DocumentManager.MdiActiveDocument;
         if (doc == null)
         {
            return;
         }
         Editor ed = doc.Editor;
         var acadLibVer = Assembly.GetExecutingAssembly().GetName().Version;
         ed.WriteMessage($"\nБиблиотека AcadLib версии {acadLibVer}");
      }
   }
}
