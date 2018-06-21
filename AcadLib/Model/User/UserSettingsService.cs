namespace AcadLib.User
{
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Windows.Media.Imaging;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using UsersEditor;
#if !Utils
    using AcadLib.User.UI;
    using AutoCAD_PIK_Manager.User;
#endif

    [PublicAPI]
    public static class UserSettingsService
    {
        public static BitmapImage LoadHomePikImage(string login, string domain)
        {
            var client = new WebClient { Credentials = new NetworkCredential("khisyametdinovvt", "Cadman03") };
            var req = $"https://home.pik.ru/api/v1.0/Employee/byLogin?login={domain}%5C{login}";
            var resId = client.DownloadString(req);
            var bytes = Encoding.Default.GetBytes(resId);
            resId = Encoding.UTF8.GetString(bytes);
            if (JsonConvert.DeserializeObject<JArray>(resId).FirstOrDefault() is JObject objId)
            {
                var id = objId["id"].ToString();
                var resImage = client.DownloadData($"https://home.pik.ru/api/v1.0/Employee/{id}/photo?renditionId=1");
                return LoadImage(resImage);
            }

            return null;
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }

            image.Freeze();
            return image;
        }
#if !Utils
        private static UsersEditorView users;

        private static string GetFileUserSettings()
        {
            return AcadLib.IO.Path.GetUserPluginFile(string.Empty, "UserSettings.json");
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

        public static void UsersEditor()
        {
            try
            {
                if (!General.IsBimUser)
                {
                    "Доступ только для BIM".WriteToCommandLine();
                    return;
                }
            }
            catch
            {
                //
            }

            if (users == null)
            {
                users = new UsersEditorView(new UsersEditorVM());
                users.Closed += (o, e) => users = null;
            }

            users.Show();
        }
#endif
    }
}