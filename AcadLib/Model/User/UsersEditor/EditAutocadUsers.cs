
using System.Windows.Media.Imaging;
#if Utils
using UtilsEditUsers.Model.User.DB;
#else
using AcadLib.Model.User.DB;
#endif
using NetLib.WPF;
using ReactiveUI;

namespace AcadLib.User.UsersEditor
{
    public class EditAutocadUsers : BaseModel
    {
        public AutocadUsers DbUser { get; set; }
        public string Login { get; set; }
        public string FIO { get; set; }
        public string AdDepartment { get; set; }
        public string AdPosition { get; set; }
        public BitmapImage Image { get; set; }
        public string Group { get; set; }
        public string Version { get; set; }
        public bool? Disabled { get; set; }
        public string Description { get; set; }

        public EditAutocadUsers()
        {
        }

        public EditAutocadUsers(AutocadUsers dbUser)
        {
            DbUser = dbUser;
            Login = dbUser.Login;
            FIO = dbUser.FIO;
            Disabled = dbUser.Disabled;
            Description = dbUser.Description;
            Group = dbUser.Group;
            Version = dbUser.Version;
        }

        public void SaveToDbUser()
        {
            DbUser.Group = Group;
            DbUser.FIO = FIO;
            DbUser.Login = Login;
            DbUser.Description = Description;
            DbUser.Disabled = Disabled ?? false;
        }

        public override string ToString()
        {
            return $"{Login}, {FIO}, {Description}, {AdDepartment}, {AdPosition}";
        }
    }
}