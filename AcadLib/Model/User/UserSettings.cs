namespace AcadLib.User
{
    using System.Collections.Generic;

    public class UserSettings
    {
        /// <summary>
        /// Настройки плагинов
        /// </summary>
        public Dictionary<string, PluginSettings> PluginSettings { get; set; }
    }

    public class PluginSettings
    {
        /// <summary>
        /// Свойства
        /// </summary>
        public List<UserProperty> Properties { get; set; }
    }

    public class UserProperty
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public string Description { get; set; }
    }
}