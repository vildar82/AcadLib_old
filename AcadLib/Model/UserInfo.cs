using MongoDblib.UsersData.Data;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AcadLib
{
    [PublicAPI]
    public static class UserInfo
    {
        public static string FioAD { get; set; }

        public static UserData UserData { get; set; }

        public static List<string> UserGroupsAd { get; set; }

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
                Logger.Log.Error(ex, "adUtils");
            }
        }

        public static void ShowUserProfileRegister()
        {
            MongoDblib.UsersData.UserDataRegUI.ShowUserProfileRegister(FioAD, "", "AutoCAD");
        }
    }
}