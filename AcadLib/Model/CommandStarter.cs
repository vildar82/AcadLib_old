using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using AcadLib.Model.Statistic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace AcadLib
{
    public class CommandStart
    {
        public static string CurrentCommand { get; private set; }
        public string CommandName { get; private set; }
        public string Plugin { get; set; }
        public string Doc { get; set; }
        public Assembly Assembly { get; private set; }

        public CommandStart () { }
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
            CommandStart commandStart = new CommandStart();
            // определение имени команды по вызвавему методу и иего артрибуту CommandMethod;            
            try
            {
                var caller = new StackTrace().GetFrame(1).GetMethod();
                CurrentCommand = GetCallerCommand(caller);
                //commandStart = new CommandStart(CurrentCommand, caller.DeclaringType.Assembly);
                commandStart.CommandName = CurrentCommand;
                commandStart.Assembly = caller.DeclaringType.Assembly;
                commandStart.Plugin = commandStart.Assembly.GetName().Name;
                Logger.Log.StartCommand(commandStart);
                CommandCounter.CountCommand(CurrentCommand);
            }
            catch { }

            Document doc = Application.DocumentManager.MdiActiveDocument;            
            if (doc == null) return;
            commandStart.Doc = doc.Name;
            Logger.Log.Info($"Document={doc.Name}");
            PluginStatisticsHelper.PluginStart(commandStart);

            Inspector.Clear();
            try
            {
                action(doc);
            }
            catch (CancelByUserException cancelByUser)
            {
                doc.Editor.WriteMessage(cancelByUser.Message);
            }
            catch (Exceptions.ErrorException error)
            {
                Inspector.AddError(error.Error);
            }
            catch (System.Exception ex)
            {
                if (!ex.Message.Contains(General.CanceledByUser))
                {
                    Logger.Log.Error(ex, CurrentCommand);
                    Inspector.AddError($"Ошибка в программе. {ex}", System.Drawing.SystemIcons.Error);
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
