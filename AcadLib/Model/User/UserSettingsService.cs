using AcadLib.User.UI;
using AcadLib.User.UsersEditor;
using AutoCAD_PIK_Manager.User;

namespace AcadLib.User
{
    public static class UserSettingsService
    {
        private static UsersEditorView users;

        public static void Show()
        {
            var userSettingsVm = new UserSettingsVM(AutocadUserService.User);
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
                users.Closed += (o,e) => users = null;
            }
            users.Show();
        }
    }
}