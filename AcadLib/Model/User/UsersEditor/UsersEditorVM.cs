using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using AcadLib.IO;
using AcadLib.User.DB;
using AutoCAD_PIK_Manager.Settings;
using NetLib;
using NetLib.Locks;
using NetLib.WPF;
using NetLib.WPF.Data;
using ReactiveUI;

namespace AcadLib.User.UsersEditor
{
    public class UsersEditorVM : BaseViewModel
    {
        private List<EditAutocadUsers> users;
        private DbUsers dbUsers;
        private FileLock fileLock;
        private string filter;

        public UsersEditorVM()
        {
            dbUsers = new DbUsers();
            users = dbUsers.GetUsers().Select(s => new EditAutocadUsers(s)).ToList();
            Users = new CollectionView<EditAutocadUsers>(users) { Filter  = OnFilter};
            Groups = PikSettings.UserGroups;
            this.WhenAnyValue(v => v.EditMode).Subscribe(ChangeMode);
            Save = CreateCommand(dbUsers.Save, this.WhenAnyValue(v=>v.EditMode));
            this.WhenAnyValue(v => v.SelectedUsers).Subscribe(s => OnSelected());
            this.WhenAnyValue(v => v.Filter).Subscribe(s => Users.Refresh());
        }

        public CollectionView<EditAutocadUsers> Users { get; set; }
        public ReactiveCommand Save { get; set; }
        public EditAutocadUsers SelectedUser { get; set; }
        public List<EditAutocadUsers> SelectedUsers { get; set; }
        public List<string> Groups { get; set; }
        public bool IsOneUserSelected { get; set; }
        public bool EditUserEnable { get; set; }
        public ReactiveCommand Apply { get; set; }
        public bool EditMode { get; set; }
        public string Filter { get; set; }

        private bool OnFilter(object obj)
        {
            if (!Filter.IsNullOrEmpty() && obj is EditAutocadUsers user)
            {
                return Regex.IsMatch(user.ToString(), Filter, RegexOptions.IgnoreCase);
            }
            return true;
        }

        private void ChangeMode(bool editMode)
        {
            if (editMode)
            {
                // Создать файл блокировки
                fileLock = new FileLock(Path.GetSharedCommonFile("UsersEditor", "UsersEditor.lock"));
                if (!fileLock.IsLockSuccess)
                {
                    ShowMessage(fileLock.GetMessage(), "Доступ заблокирован");
                    EditMode = false;
                }
            }
            else
            {
                fileLock?.Dispose();
            }
        }

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
                FIO = GetValue(u => u.FIO),
                Login = GetValue(u => u.Login),
                Group = GetValue(u => u.Group),
                Disabled = GetValue(u => u.Disabled),
                Description = GetValue(u => u.Description),
            };
            Apply = CreateCommand(() => ApplyExecute(SelectedUser, SelectedUsers),
                SelectedUser.Changed.Skip(1).Select(s => true));
        }

        private void ApplyExecute(EditAutocadUsers selectedUser, List<EditAutocadUsers> selectedUsers)
        {
            foreach (var autocadUserse in selectedUsers)
            {
                autocadUserse.Group = selectedUser.Group;
                autocadUserse.Description = selectedUser.Description;
                autocadUserse.Disabled = selectedUser.Disabled;
                autocadUserse.SaveToDbUser();
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
}