using MongoDblib.UsersData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    LoadService.LoadMongoDb();
                    UserGroupsAd = adUtils.GetCurrentUserGroups(out string fioAd);
                    FioAD = fioAd;
                }
                UserData = new MongoDblib.UsersData.DbUserData().GetCurrentUser();
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex,$"adUtils");
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
