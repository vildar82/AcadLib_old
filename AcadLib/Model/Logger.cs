using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib
{
    public static class Logger
    {
        public static LoggAddinExt Log;
        public static string UserGroup = AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup;

        static Logger ()
        {
            Log = new LoggAddinExt(UserGroup);            
        }        
    }

    public class LoggAddinExt : AutoCAD_PIK_Manager.LogAddin
    {
        public LoggAddinExt (string plugin) : base(plugin)
        {
        }

        /// <summary>
        /// Отзыв
        /// </summary>        
        public void Report (string msg)
        {
            Error("#Report: " + msg);
        }

        public new void Error (string msg)
        {
            var newMsg = GetMessage(msg);
            base.Error(newMsg);
        }

        public void Error (Exception ex, string msg)
        {
            var newMsg = GetMessage(msg);
            base.Error(ex, newMsg);
        }

        public new void Info (string msg)
        {
            var newMsg = GetMessage(msg);
            base.Info(newMsg);
        }
        public void Info (Exception ex, string msg)
        {
            var newMsg = GetMessage(msg);
            base.Info(ex, newMsg);            
        }

        public new void Debug (string msg)
        {
            var newMsg = GetMessage(msg);
            base.Debug(newMsg);
        }       

        public void Debug (Exception ex, string msg)
        {
            var newMsg = GetMessage(msg);
            base.Debug(ex, newMsg);
        }

        public new void Fatal (string msg)
        {
            var newMsg = GetMessage(msg);
            base.Fatal(newMsg);
        }
        public void Fatal (Exception ex, string msg)
        {
            var newMsg = GetMessage(msg);
            base.Fatal(ex, newMsg);
        }

        public new void Warn (string msg)
        {
            var newMsg = GetMessage(msg);
            base.Warn(newMsg);
        }
        public void Warn (Exception ex, string msg)
        {
            var newMsg = GetMessage(msg);
            base.Warn(ex, newMsg);
        }

        public void StartCommand (CommandStart command)
        {
            base.Info($"Start command: {command.CommandName}; Сборка: {command.Assembly.FullName}; ");
        }

        public void StartLisp (string command, string file)
        {
            base.Info($"Start Lisp: {command}; Файл: {file}; ");
        }

        private string GetMessage (string msg)
        {
            return $"Команда: {CommandStart.CurrentCommand}; Сообщение: {msg}";
        }
    }
}
