using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace AcadLib
{
    public static class CommandStart
    {
        public static void Start(string name, Action<Document> action)
        {
            Logger.Log.StartCommand(name);
            CommandCounter.StartCommand(name);
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            try
            {
                action(doc);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains(General.CanceledByUser))
                {
                    Logger.Log.Error(ex, name);
                }
                doc.Editor.WriteMessage(ex.Message);
            }
        }
    }
}
