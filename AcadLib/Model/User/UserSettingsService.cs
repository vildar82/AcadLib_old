namespace AcadLib.User
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoCAD_PIK_Manager.User;
    using IO;
    using JetBrains.Annotations;
    using NetLib;
    using PaletteProps;
    using UI;

    /// <summary>
    /// Настройки пользователя
    /// </summary>
    [PublicAPI]
    public static class UserSettingsService
    {
        internal const string CommonName = "Общие";
        internal const string CommonParamNotify = "NotificationsOn";
        [NotNull] private static LocalFileData<UserSettings> _userData;

        static UserSettingsService()
        {
            _userData = new LocalFileData<UserSettings>(Path.GetUserPluginFile(string.Empty, "UserSettings.json"), false);
            _userData.TryLoad(() => new UserSettings());
            CheckSettings();
            CommonSettings = GetCommonSettings();
        }

        /// <summary>
        /// Событие изменения настроек
        /// </summary>
        public static event EventHandler ChangeSettings;

        /// <summary>
        /// Общие настройки
        /// </summary>
        public static PluginSettings CommonSettings { get; set; }

        /// <summary>
        /// Пользователь согласен на предварительные обновления
        /// </summary>
        public static bool IsPreviewUpdate => AutocadUserService.User?.PreviewUpdate ?? false;

        /// <summary>
        /// Получение значения настройки плагина
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="pluginName">Имя плагина</param>
        /// <param name="parameterId">Имя параметра</param>
        public static T GetPluginValue<T>([NotNull] string pluginName, [NotNull] string parameterId)
        {
            var prop = GetPluginProperty(pluginName, parameterId);
            if (prop == null)
                return default;
            return (T)prop.Value;
        }

        /// <summary>
        /// Получение свойтва плагина
        /// </summary>
        /// <param name="pluginName">Плагин</param>
        /// <param name="parameterId">Свойство</param>
        public static UserProperty GetPluginProperty([NotNull] string pluginName, [NotNull] string parameterId)
        {
            var plugin = GetPluginSettings(pluginName);
            return plugin?.Properties.FirstOrDefault(p => p.ID == parameterId);
        }

        /// <summary>
        /// Установить значение свойства
        /// </summary>
        /// <param name="pluginName">Плагин</param>
        /// <param name="parameterId">Параметр</param>
        /// <param name="value">Значение</param>
        public static void SetPluginValue([NotNull] string pluginName, [NotNull] string parameterId, object value)
        {
            var plugin = GetPluginSettings(pluginName);
            var prop = plugin?.Properties.FirstOrDefault(p => p.ID == parameterId);
            if (prop == null)
                return;
            if (!Equals(value, prop.Value))
            {
                prop.Value = value;
                _userData.TrySave();
            }
        }

        /// <summary>
        /// Получение настроек плагина
        /// </summary>
        /// <param name="name">Имя плагина</param>
        [CanBeNull]
        public static PluginSettings GetPluginSettings([NotNull] string name)
        {
            return _userData.Data.PluginSettings.FirstOrDefault(p => p?.Name == name);
        }

        /// <summary>
        /// Добавление пользовательских настроек плагина
        /// </summary>
        /// <param name="pluginName">Плагин</param>
        [NotNull]
        public static PluginSettings AddPluginSettings([NotNull] string pluginName)
        {
            var plugin = GetPluginSettings(pluginName);
            if (plugin != null)
            {
                Logger.Log.Error($"Настройки такого плагина уже есть - '{pluginName}'.");
                return plugin;
            }

            plugin = new PluginSettings { Name = pluginName };
            _userData.Data.PluginSettings.Add(plugin);
            return plugin;
        }

        public static void RemovePlugin([NotNull] string pluginName)
        {
            _userData.Data.PluginSettings.RemoveAll(p => p.Name == pluginName);
        }

        /// <summary>
        /// Показать настройки пользователя
        /// </summary>
        public static void Show()
        {
            var user = AutocadUserService.LoadUser();
            if (user == null)
            {
                "Ошибка загрузки пользователя из базы. Загрузка из локального кеша.".WriteToCommandLine();
                user = AutocadUserService.LoadBackup();
                if (user == null)
                {
                    "Ошибка загрузки пользователя из локального кеша.".WriteToCommandLine();
                }
            }

            InitControls();

            var userSettingsVm = new UserSettingsVM(user, _userData.Data);
            var userSettingsView = new UserSettingsView(userSettingsVm);
            if (userSettingsView.ShowDialog() != true)
                return;
            AutocadUserService.User = userSettingsVm.User;
            AutocadUserService.Save();
            _userData.TrySave();
            ChangeSettings?.Invoke(null, EventArgs.Empty);
        }

        private static void InitControls()
        {
            foreach (var pluginSetting in _userData.Data.PluginSettings)
            {
                foreach (var property in pluginSetting.Properties)
                {
                    if (property.ValueControl == null)
                    {
                        property.ValueControl = property.Value.CreateControl(v =>
                            property.Value = v);
                    }
                }
            }
        }

        private static void CheckSettings()
        {
            var incorrectPlugins = new List<PluginSettings>();
            foreach (var pluginSetting in _userData.Data.PluginSettings)
            {
                if (pluginSetting.Name.IsNullOrEmpty() || !pluginSetting.Properties.Any())
                {
                    incorrectPlugins.Add(pluginSetting);
                    continue;
                }

                var incorrectProps = new List<UserProperty>();
                foreach (var property in pluginSetting.Properties)
                {
                    if (property.ID.IsNullOrEmpty())
                    {
                        incorrectProps.Add(property);
                    }
                }

                incorrectProps.ForEach(r => pluginSetting.Properties.Remove(r));
                if (pluginSetting.Properties.Count == 0)
                {
                    incorrectPlugins.Add(pluginSetting);
                }
            }

            incorrectPlugins.ForEach(p => _userData.Data.PluginSettings.Remove(p));
        }

        private static PluginSettings GetCommonSettings()
        {
            var common = GetPluginSettings(CommonName);
            if (common == null)
            {
                common = AddPluginSettings(CommonName);
                AddNotifyProp();
            }
            else
            {
                // Есть ли свойство - Уведомления
                common.GetPluginValue(CommonParamNotify, AddNotifyProp);
            }

            bool AddNotifyProp()
            {
                common.Add(CommonParamNotify,
                    "Уведомления",
                    "Включение/отключение всплывающих уведомлений об изменении настроек",
                    true);
                return true;
            }

            return common;
        }
    }
}