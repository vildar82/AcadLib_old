#if Utils
using UtilsEditUsers.Model.User.DB;
#else
using AcadLib.Model.User.DB;
#endif
using NetLib.WPF;

namespace AcadLib.User.UsersEditor
{
    public class EditAutocadUsers : BaseModel
    {
        public AutocadUsers DbUser { get; set; }
        public string Login { get; set; }
        public string FIO { get; set; }
        public string Group { get; set; }
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
            return $"{Login}, {FIO}, {Group}, {Description}";
        }
    }
}