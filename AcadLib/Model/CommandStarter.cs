using AcadLib.Errors;
using AcadLib.Statistic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using AcadLib.CommandLock;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib
{
    [PublicAPI]
    public class CommandStart
    {
        public static string CurrentCommand { get; set; }
        public string CommandName { get; private set; }
        public string Plugin { get; set; }
        public string Doc { get; set; }
        public Assembly Assembly { get; private set; }

        public CommandStart()
        {
        }

        public CommandStart(string commandName, Assembly asm)
        {
            CommandName = commandName;
            Assembly = asm;
        }

        public static void StartLisp(string commandName, string file)
        {
            Logger.Log.StartLisp(commandName, file);
            PluginStatisticsHelper.PluginStart(new CommandStart(commandName, null) { Doc = file, Plugin = "Lisp" });
        }

        public static void Start(string commandName, Action<Document> action)
        {
            MethodBase caller = null;
            try
            {
                caller = new StackTrace().GetFrame(1).GetMethod();
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, "CommandStart - StackTrace");
            }
            StartCommand(action, caller, commandName);
        }

        /// <summary>
        /// Оболочка для старта команды - try-catch, log, inspectoe.clear-show, commandcounter
        /// Условие использования: отключить оптимизацию кода (Параметры проекта -> Сборка) - т.к. используется StackTrace
        /// </summary>
        /// <param name="action">Код выполнения команды</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Start(Action<Document> action)
        {
            MethodBase caller = null;
            try
            {
                caller = new StackTrace().GetFrame(1).GetMethod();
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, "CommandStart - StackTrace");
            }
            StartCommand(action, caller, null);
        }

        public static void StartWoStat(Action<Document> action)
        {
            MethodBase caller = null;
            try
            {
                caller = new StackTrace().GetFrame(1).GetMethod();
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, "CommandStart - StackTrace");
            }
            StartCommand(action, caller, null, true);
        }

        public static void Start(Action<Document> action, Version minAcadVersion)
        {
            if (Application.Version < minAcadVersion)
            {
                MessageBox.Show($"Команда не работает в данной версии автокада. \nМинимальная требуемая версия {minAcadVersion}.");
                return;
            }
            MethodBase caller = null;
            try
            {
                caller = new StackTrace().GetFrame(1).GetMethod();
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex, "CommandStart - StackTrace");
            }
            StartCommand(action, caller, null);
        }

        private static void StartCommand(Action<Document> action, MethodBase caller, string commandName, bool woStatistic = false)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            if (!woStatistic)
            {
                try
                {
                    var commandStart = GetCallerCommand(caller, commandName);
                    Logger.Log.StartCommand(commandStart);
                    Logger.Log.Info($"Document={doc.Name}");
                    PluginStatisticsHelper.PluginStart(commandStart);
                    // Проверка блокировки команды
                    if (!CommandLockService.CanStartCommand(commandStart.CommandName))
                    {
                        Logger.Log.Info($"Команда заблокирована - {commandStart.CommandName}");
                        return;
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Log.Error(ex, "CommandStart");
                }
            }
            try
            {
#pragma warning disable 618
                Inspector.Clear();
#pragma warning restore 618
                action(doc);
            }
            catch (OperationCanceledException ex)
            {
                doc.Editor.WriteMessage(ex.Message);
            }
#pragma warning disable 618
            catch (CancelByUserException cancelByUser)
#pragma warning restore 618
            {
                doc.Editor.WriteMessage(cancelByUser.Message);
            }
            catch (Exceptions.ErrorException error)
            {
                Inspector.AddError(error.Error);
            }
            catch (System.Exception ex)
            {
#pragma warning disable 612
                if (!ex.Message.Contains(General.CanceledByUser))
#pragma warning restore 612
                {
                    Logger.Log.Error(ex, CurrentCommand);
                    Inspector.AddError($"Ошибка в программе. {ex.Message}", System.Drawing.SystemIcons.Error);
                }
                doc.Editor.WriteMessage(ex.Message);
            }
            Inspector.Show();
        }

        [NotNull]
        internal static CommandStart GetCallerCommand([CanBeNull] MethodBase caller, [CanBeNull] string commandName = null)
        {
            Assembly assm = null;
            try
            {
                CurrentCommand = commandName ?? GetCallerCommandName(caller);
                assm = caller?.DeclaringType?.Assembly;
            }
            catch
            {
                //
            }
            var com = new CommandStart
            {
                CommandName = CurrentCommand,
                Assembly = assm,
                Plugin = assm?.GetName().Name,
                Doc = Application.DocumentManager.MdiActiveDocument?.Name
            };
            return com;
        }

        [NotNull]
        private static string GetCallerCommandName([CanBeNull] MethodBase caller)
        {
            if (caller == null) return "nullCallerMethod!?";
            var atrCom = (CommandMethodAttribute)caller.GetCustomAttribute(typeof(CommandMethodAttribute));
            return atrCom?.GlobalName ?? caller.Name;
        }
    }
}