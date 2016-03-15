using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(AcadLib.Commands))]

namespace AcadLib
{
    public class Commands
    {
        [CommandMethod("PIK", "PIK-Acadlib-About", CommandFlags.Modal)]
        public void About()
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

        [CommandMethod("PIK", "DbObjectsCountInfo", CommandFlags.Modal)]
        public void DbObjectsCountInfo()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            Dictionary<string, int> allTypes = new Dictionary<string, int>();

            for (long i = db.BlockTableId.Handle.Value; i < db.Handseed.Value; i++)
            {
                ObjectId id;
                if (db.TryGetObjectId(new Handle(i), out id))
                {
                    if (allTypes.ContainsKey(id.ObjectClass.Name))
                    {
                        allTypes[id.ObjectClass.Name]++;
                    }
                    else
                    {
                        allTypes.Add(id.ObjectClass.Name, 1);
                    }
                }
            }

            var sortedByCount = allTypes.OrderBy(i => i.Value);

            foreach (var item in sortedByCount)
            {
                ed.WriteMessage($"\n{item.Key} - {item.Value}");
            }
        }

        [CommandMethod("PIK", "PIK-BlockList", CommandFlags.Modal)]
        public void BlockListCommand()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            try
            {
                BlockList.List(doc.Database);
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage($"\nОшибка - {ex.Message}");
            }            
        }
    }
}
