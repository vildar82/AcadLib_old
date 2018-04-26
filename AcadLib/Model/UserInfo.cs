using MongoDblib.UsersData.Data;
using System;
using System.Collections.Generic;
using AcadLib.IO;
using JetBrains.Annotations;
using NetLib;

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
                SaveBackup();
            }
            catch (Exception ex)
            {
                LoadBackup();
                Logger.Log.Error(ex, "adUtils");
            }
        }

        private static void SaveBackup()
        {
            try
            {
                var user = new UserInfoData
                {
                    UserData = UserData,
                    FioAD = FioAD,
                    UserGroupsAd = UserGroupsAd
                };
                var file = GetFile();
                user.Serialize(file);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "UserInfo SaveBackup");
            }
        }

        private static void LoadBackup()
        {
            try
            {
                var file = GetFile();
                var user = file.Deserialize<UserInfoData>();
                FioAD = user.FioAD;
                UserData = user.UserData;
                UserGroupsAd = user.UserGroupsAd;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "UserInfo LoadBackup");
            }
        }

        [NotNull]
        private static string GetFile()
        {
            return Path.GetUserPluginFile("UserInfo", "UserInfo");
        }

        public static void ShowUserProfileRegister()
        {
            MongoDblib.UsersData.UserDataRegUI.ShowUserProfileRegister(FioAD, "", "AutoCAD");
        }
    }

    public class UserInfoData
    {
        public string FioAD { get; set; }
        public UserData UserData { get; set; }
        public List<string> UserGroupsAd { get; set; }
    }
}