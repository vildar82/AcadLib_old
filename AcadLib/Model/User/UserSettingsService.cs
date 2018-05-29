using AcadLib.User.UI;
using AutoCAD_PIK_Manager.User;

namespace AcadLib.User
{
    public static class UserSettingsService
    {
        public static void Show()
        {
            var userSettingsVm = new UserSettingsVM(AutocadUserService.User);
            var userSettingsView = new UserSettingsView(userSettingsVm);
            if (userSettingsView.ShowDialog() != true) return;
            AutocadUserService.User = userSettingsVm.User;
            AutocadUserService.Save();
        }
    }
}