using MongoDblib.UsersData.Data;
using System;
using System.Collections.Generic;

namespace AcadLib
{
    public static class UserInfo
    {
        static UserInfo()
        {
            try
            {
                using (var adUtils = new NetLib.AD.ADUtils())
                {
                    UserGroupsAd = adUtils.GetCurrentUserGroups(out var fioAd);
                    FioAD = fioAd;
                }
                UserData = new MongoDblib.UsersData.DbUserData().GetCurrentUser();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"adUtils");
            }
        }

        public static UserData UserData { get; set; }
        public static string FioAD { get; set; }
        public static List<string> UserGroupsAd { get; set; }

        public static void ShowUserProfileRegister()
        {
            MongoDblib.UsersData.UserDataRegUI.ShowUserProfileRegister(FioAD, "", "AutoCAD");
        }
    }
}
