using AcadLib.User.UI;
using AcadLib.User.UsersEditor;
using AutoCAD_PIK_Manager.User;
using JetBrains.Annotations;
using Path = AcadLib.IO.Path;

namespace AcadLib.User
{
    [PublicAPI]
    public static class UserSettingsService
    {
        private static UsersEditorView users;

        private static string GetFileUserSettings()
        {
            return Path.GetUserPluginFile("", "UserSettings.json");
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
            if (userSettingsView.ShowDialog() != true) return;
            AutocadUserService.User = userSettingsVm.User;
            AutocadUserService.Save();
        }

        public static void UsersEditor()
        {
            if (!General.IsBimUser)
            {
                "Доступ только для BIM".WriteToCommandLine();
                return;
            }
            if (users == null)
            {
                users = new UsersEditorView(new UsersEditorVM());
                users.Closed += (o, e) => users = null;
            }
            users.Show();
        }
    }
}