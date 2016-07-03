using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace AcadLib
{
    public static class CommandStart
    {
        /// <summary>
        /// Оболочка для старта команды - try-catch, log, inspectoe.clear-show, commandcounter
        /// Условие использования: отключить оптимизацию кода (Параметры проекта -> Сборка) - т.к. используется StackTrace
        /// </summary>
        /// <param name="action">Код выполнения команды</param>
        [MethodImpl(MethodImplOptions.NoInlining)]        
        public static void Start(Action<Document> action)
        {
            // определение имени команды по вызвавему методу и иего артрибуту CommandMethod;
            string command = string.Empty;
            try
            {                
                var caller = new StackTrace().GetFrame(1).GetMethod();
                command = GetCallerCommand(caller);
            }
            catch { }      

            Logger.Log.StartCommand(command);
            CommandCounter.CountCommand(command);
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Inspector.Clear();
            try
            {
                action(doc);
            }
            catch (System.Exception ex)
            {
                if (!ex.Message.Contains(General.CanceledByUser))
                {
                    Logger.Log.Error(ex, command);
                }
                else
                {
                    Inspector.AddError($"Ошибка в программе. {ex}");
                }
                doc.Editor.WriteMessage(ex.Message);                
            }
            Inspector.Show();
        }

        private static string GetCallerCommand(MethodBase caller)
        {
            if (caller == null) return "nullCallerMethod!?";
            var atrCom = (CommandMethodAttribute)caller.GetCustomAttribute(typeof(CommandMethodAttribute));            
            return atrCom?.GlobalName;
        }
    }
}
