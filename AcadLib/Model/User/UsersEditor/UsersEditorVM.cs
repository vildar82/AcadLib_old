using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AcadLib.Model.User.DB;
using AcadLib.User.DB;
using AutoCAD_PIK_Manager.Settings;
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
            Users = dbUsers.GetUsers().Select(s=> new EditAutocadUsers(s)).ToList();
            Groups = PikSettings.UserGroups;
            //Save = CreateCommand(dbUsers.Save);
            this.WhenAnyValue(v => v.SelectedUsers).Subscribe(s => OnSelected());
        }

        public List<EditAutocadUsers> Users { get; set; }
        public ReactiveCommand Save { get; set; }
        public EditAutocadUsers SelectedUser { get; set; }
        public List<EditAutocadUsers> SelectedUsers { get; set; }
        public List<string> Groups { get; set; }
        public bool IsOneUserSelected { get; set; }
        public bool EditUserEnable { get; set; }
        public ReactiveCommand Apply { get; set; }

        private void OnSelected()
        {
            if (SelectedUsers == null)
            {
                SelectedUser = null;
                EditUserEnable = false;
                return;
            }
            EditUserEnable = General.IsBimUser;
            IsOneUserSelected = SelectedUsers.Count == 1;
            SelectedUser = new EditAutocadUsers
            {
                FIO = GetValue(u=>u.FIO),
                Login = GetValue(u=>u.Login),
                Group = GetValue(u=>u.Group),
                Disabled = GetValue(u=>u.Disabled),
                Description = GetValue(u=>u.Description),
            };
            Apply = CreateCommand(()=>ApplyExecute(SelectedUser, SelectedUsers), SelectedUser.Changed.Skip(1).Select(s => true));
        }

        private void ApplyExecute(EditAutocadUsers selectedUser, List<EditAutocadUsers> selectedUsers)
        {
            foreach (var autocadUserse in selectedUsers)
            {
                autocadUserse.Group = selectedUser.Group;
                autocadUserse.Description = selectedUser.Description;
                autocadUserse.Disabled = selectedUser.Disabled;
            }
            if (selectedUsers.Count == 1)
            {
                var user = selectedUsers[0];
                user.FIO = selectedUser.FIO;
                user.Login = selectedUser.Login;
            }
        }

        private T GetValue<T>(Func<EditAutocadUsers, T> prop)
        {
            var res = SelectedUsers.GroupBy(prop).Select(s => s.Key);
            var moreOne = res.Skip(1).Any();
            var value = res.First();
            return moreOne ? default : value;
        }
    }

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
    }
}
