namespace AcadLib.User
{
    using AutoCAD_PIK_Manager.User;
    using JetBrains.Annotations;
    using UI;

    [PublicAPI]
    public static class UserSettingsService
    {
        private static string GetFileUserSettings()
        {
            return IO.Path.GetUserPluginFile(string.Empty, "UserSettings.json");
        }

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

            var userSettingsVm = new UserSettingsVM(user);
            var userSettingsView = new UserSettingsView(userSettingsVm);
            if (userSettingsView.ShowDialog() != true)
                return;
            AutocadUserService.User = userSettingsVm.User;
            AutocadUserService.Save();
        }
    }
}