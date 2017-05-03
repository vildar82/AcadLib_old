using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using AcadLib.Errors;
using AcadLib.Statistic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib
{
    public class CommandStart
    {
        public static string CurrentCommand { get; set; }
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
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            try
            {
                var caller = new StackTrace().GetFrame(1).GetMethod();
                var commandStart = GetCallerCommand(caller);
                Logger.Log.StartCommand(commandStart);                
                Logger.Log.Info($"Document={doc.Name}");
                PluginStatisticsHelper.PluginStart(commandStart);

                Inspector.Clear();
            }
            catch { }
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
                    Inspector.AddError($"Ошибка в программе. {ex.Message}", System.Drawing.SystemIcons.Error);
                }
                doc.Editor.WriteMessage(ex.Message);
            }
            Inspector.Show();
        }

        internal static CommandStart GetCallerCommand(MethodBase caller)
        {            
            Assembly assm = null;
            try
            {                
                CurrentCommand = GetCallerCommandName(caller);
                assm = caller?.DeclaringType.Assembly;
            }
            catch { }
            var com = new CommandStart()
            {
                CommandName = CurrentCommand,
                Assembly = assm,
                Plugin = assm?.GetName().Name,
                Doc = Application.DocumentManager.MdiActiveDocument?.Name
            };
            return com;
        }

        private static string GetCallerCommandName(MethodBase caller)
        {            
            if (caller == null) return "nullCallerMethod!?";            
            var atrCom = (CommandMethodAttribute)caller.GetCustomAttribute(typeof(CommandMethodAttribute));
            return atrCom?.GlobalName ?? caller.Name;                        
        }
    }
}
