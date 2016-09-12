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
    public class CommandStart
    {
        public static string CurrentCommand { get; private set; }
        public string CommandName { get; private set; }
        public Assembly Assembly { get; private set; }

        public CommandStart(string commandName, Assembly asm)
        {
            CommandName = commandName;
            Assembly = asm;
        }
        
        public static void StartLisp (string commandName, string file)
        {
            Logger.Log.StartLisp(commandName, file);
        }

        /// <summary>
        /// Оболочка для старта команды - try-catch, log, inspectoe.clear-show, commandcounter
        /// Условие использования: отключить оптимизацию кода (Параметры проекта -> Сборка) - т.к. используется StackTrace
        /// </summary>
        /// <param name="action">Код выполнения команды</param>
        [MethodImpl(MethodImplOptions.NoInlining)]        
        public static void Start(Action<Document> action)
        {
            // определение имени команды по вызвавему методу и иего артрибуту CommandMethod;            
            try
            {
                var caller = new StackTrace().GetFrame(1).GetMethod();
                CurrentCommand = GetCallerCommand(caller);
                var command = new CommandStart(CurrentCommand, caller.DeclaringType.Assembly);
                Logger.Log.StartCommand(command);
                CommandCounter.CountCommand(CurrentCommand);
            }
            catch { }

            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Inspector.Clear();
            try
            {
                action(doc);
            }
            catch (CancelByUserException cancelByUser)
            {
                doc.Editor.WriteMessage(cancelByUser.Message);
            }
            catch (System.Exception ex)
            {
                if (!ex.Message.Contains(General.CanceledByUser))
                {
                    Logger.Log.Error(ex, CurrentCommand);
                    Inspector.AddError($"Ошибка в программе. {ex.Message}", System.Drawing.SystemIcons.Error);
                }
                doc.Editor.WriteMessage(ex.Message);
            }
            Inspector.Show();
        }

        private static string GetCallerCommand(MethodBase caller)
        {            
            if (caller == null) return "nullCallerMethod!?";
            string name = string.Empty;
            var atrCom = (CommandMethodAttribute)caller.GetCustomAttribute(typeof(CommandMethodAttribute));            
            if (atrCom != null)
            {
                name = atrCom.GlobalName;
            }
            else
            {
                name = caller.Name;
            }
            return name;
        }
    }
}
