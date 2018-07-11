namespace AcadLib.Statistic
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using AcadLib;
    using FileLog.Client;
    using FileLog.Entities;
    using JetBrains.Annotations;
    using Naming.Dto;
    using NetLib.AD;
    using PathChecker;

    /// <summary>
    /// Класс для отправки событий
    /// </summary>
    public class Eventer
    {
        private readonly FlClient _client;
        private readonly PathChecker _pathChecker;
        private UserData _userData;

        private string App { get; }

        private AppType AppType { get; }

        private DateTime StartEvent { get; set; }

        private string Version { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="app">Имя приложения</param>
        /// <param name="version">Версия приложения</param>
        public Eventer(string app, string version)
        {
            App = app;
            AppType = GetAppType(app);
            Version = version;
            _client = new FlClient();
            _pathChecker = new PathChecker(_client);
        }

        /// <summary>
        /// Конец события
        /// </summary>
        /// <param name="eventName">Имя события</param>
        public void Finish(string eventName, string docPath, string serialNumber)
        {
            if (string.IsNullOrEmpty(docPath) || !File.Exists(docPath))
            {
                return;
            }

            var eventEnd = DateTime.Now;
            Task.Run(
                () =>
                {
                    try
                    {
                        var fileName = Path.GetFileNameWithoutExtension(docPath);
                        var userName = Environment.UserName;
                        var compName = Environment.MachineName;
                        if (_userData == null)
                            _userData = GetUserDataAd();
                        var fi = new FileInfo(docPath);
                        var fileSize = fi.Length / 1024000;

                        var eventTimeSec = (int)(eventEnd - StartEvent).TotalSeconds;

                        _client.AddEvent(
                            new StatEvent(
                                App,
                                userName,
                                compName,
                                fileName,
                                docPath,
                                eventName,
                                StartEvent,
                                eventEnd,
                                Version,
                                fileSize,
                                eventTimeSec,
                                serialNumber,
                                _userData?.Fio,
                                _userData?.Department,
                                _userData?.Position));
                    }
                    catch (Exception e)
                    {
                        Logger.Log.Error(e, $"Finish docPath={docPath}");
                    }
                });
        }

        /// <summary>
        /// Начало события
        /// </summary>
        /// <param name="case">Кейс</param>
        /// <param name="docPath">Документ</param>
        public CheckResultDto Start([CanBeNull] string @case, [CanBeNull] string docPath)
        {
            CheckResultDto checkResultDto = null;
            if (docPath != null && !General.IsBimUser)
            {
                try
                {
                    checkResultDto = _pathChecker.Check(AppType, @case, docPath,
                        @"https://pikipedia.pik.ru/wiki/Правила_именования_папок_и_файлов_на_WIP");
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, $"Start case={@case}, doc={docPath}");
                }
            }

            StartEvent = DateTime.Now;

            return checkResultDto;
        }

        private static UserData GetUserDataAd()
        {
            try
            {
                return ADUtils.GetUserData(Environment.UserName, Environment.UserDomainName);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"GetUserDataAD {Environment.UserName}_{Environment.UserDomainName}");
                return null;
            }
        }

        private static AppType GetAppType(string app)
        {
            switch (app.ToLower())
            {
                case "autocad": return AppType.Autocad;
                case "civil": return AppType.Civil;
            }

            return AppType.Autocad;
        }
    }
}