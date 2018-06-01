using System.Collections.Generic;
using AcadLib.Model.User.DB;
using AcadLib.User.DB;
using NetLib.WPF;
using ReactiveUI;

namespace AcadLib.User.UsersEditor
{
    public class UsersEditorVM : BaseViewModel
    {
        private DbUsers dbUsers;

        public UsersEditorVM()
        {
            dbUsers = new DbUsers();
            Users = dbUsers.GetUsers();
            Save = CreateCommand(dbUsers.Save);
        }

        public List<AutocadUsers> Users { get; set; }
        public ReactiveCommand Save { get; set; }
    }
}
