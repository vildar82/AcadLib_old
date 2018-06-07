using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AcadLib.User.DB;
#if !Utils
using AcadLib.IO;
using AutoCAD_PIK_Manager.Settings;
using AcadLib.Model.User.DB;
#endif
using NetLib;
using NetLib.AD;
using NetLib.Locks;
using NetLib.Monad;
using NetLib.WPF;
using NetLib.WPF.Data;
using ReactiveUI;

namespace AcadLib.User.UsersEditor
{
    public class UsersEditorVM : BaseViewModel
    {
        private ConcurrentDictionary<string, (string dep, string pos, BitmapImage img)> dictUsersEx =
            new ConcurrentDictionary<string, (string dep, string pos, BitmapImage img)>();
        private List<EditAutocadUsers> users;
        private DbUsers dbUsers;
        private FileLock fileLock;
        private readonly BitmapImage imageNo;
        private IObservable<bool> canEdit;

        public UsersEditorVM()
        {
#if Utils
            var groups = ADUtils.GetCurrentUserADGroups(out _);
            IsBimUser = groups.Any(g => g.EqualsIgnoreCase("010583_Отдел разработки и автоматизации") ||
                                    g.EqualsIgnoreCase("010596_Отдел внедрения ВIM") ||
                                    g.EqualsIgnoreCase("010576_УИТ"));
#else
            IsBimUser = General.IsBimUser;
#endif

            imageNo = new BitmapImage(new Uri("pack://application:,,,/Resources/no-user.png"));
            dbUsers = new DbUsers();
            this.WhenAnyValue(v => v.EditMode).Subscribe(ChangeMode);
            this.WhenAnyValue(v => v.SelectedUsers).Subscribe(s => OnSelected());
            this.WhenAnyValue(v => v.Filter, v=>v.FilterGroup).Skip(1).Subscribe(s => Users.Refresh());
            canEdit = this.WhenAnyValue(v => v.EditMode);
            Save = CreateCommand(dbUsers.Save, canEdit);
            FindMe = CreateCommand(() => Filter = Environment.UserName);
            DeleteUser = CreateCommand<EditAutocadUsers>(DeleteUserExec, canEdit);
            LoadUsers();
        }

        public bool IsBimUser { get; set; }
        public CollectionView<EditAutocadUsers> Users { get; set; }
        public ReactiveCommand Save { get; set; }
        public EditAutocadUsers SelectedUser { get; set; }
        public List<EditAutocadUsers> SelectedUsers { get; set; }
        public List<string> Groups { get; set; }
        public bool IsOneUserSelected { get; set; }
        public ReactiveCommand Apply { get; set; }
        public ReactiveCommand DeleteUser { get; set; }
        public bool EditMode { get; set; }
        public string Filter { get; set; }
        public ReactiveCommand FindMe { get; set; }
        public List<string> FilterGroups { get; set; }
        public string FilterGroup { get; set; }
        public int UsersCount { get; set; }

        private void LoadUsers()
        {
            users = dbUsers.GetUsers().Select(s => new EditAutocadUsers(s)).ToList();
            Users = new CollectionView<EditAutocadUsers>(users) { Filter  = OnFilter};
            Users.CollectionChanged += (o,e) => UsersCount = Users.Count();
#if Utils
            Groups = LoadUserGroups();
#else
            Groups = PikSettings.UserGroups;
#endif
            LoadUsersEx();
            FilterGroups = users.SelectMany(s => GetGroups(s.Group)).GroupBy(g=>g).Select(s=>s.Key).OrderBy(o=>o).ToList();
            FilterGroups.Insert(0, "Все");
        }

        private IEnumerable<string> GetGroups(string userGroup)
        {
            if (userGroup.Contains(','))
            {
                foreach (var s in userGroup.Split(',').Select(a => a.Trim())) yield return s;
            }
            else
            {
                yield return userGroup;
            }
        }

        private void LoadUsersEx()
        {
            Task.Run(() =>
            {
                Parallel.ForEach(users, u =>
                {
                    if (!dictUsersEx.TryGetValue(u.Login, out var exUser))
                    {
                        var uData = u.Login.Try(l => ADUtils.GetUserData(l, null));
                        var dep  = uData?.Department ?? "не определено";
                        var pos = uData?.Position ?? "не определено";
                        var image = u.Login.Try(l => UserSettingsService.LoadHomePikImage(l, "main")) ?? imageNo;
                        exUser =(dep, pos, image);
                        dictUsersEx.TryAdd(u.Login, exUser);
                    }
                    u.AdDepartment = exUser.dep;
                    u.AdPosition = exUser.dep;
                    u.Image = exUser.img;
                });
            });
        }

        private bool OnFilter(object obj)
        {
            var res = true;
            if (obj is EditAutocadUsers user)
            {
                if (!Filter.IsNullOrEmpty()) res = Regex.IsMatch(user.ToString(), Filter, RegexOptions.IgnoreCase);
                if (!res) return false;
                if (!FilterGroup.IsNullOrEmpty() && FilterGroup != "Все")
                {
                    if (user.Group.Contains(','))
                    {
                        res = user.Group.Split(',').Any(a => a.Trim() == FilterGroup);
                    }
                    else
                    {
                        res = user.Group == FilterGroup;
                    }
                }
            }
            return res;
        }

        private static List<string> LoadUserGroups()
        {
            var stringList = new List<string>();
            try
            {
                const string file = @"\\picompany.ru\pikp\lib\_CadSettings\AutoCAD_server\Адаптация\Общие\Dll\groups.json";
                stringList = file.Deserialize<Dictionary<string, string>>().Keys.ToList();
            }
            catch
            {
                //
            }
            return stringList;
        }

        private void ChangeMode(bool editMode)
        {
            if (editMode)
            {
                // Создать файл блокировки
#if Utils
                const string file = @"\\picompany.ru\pikp\lib\_CadSettings\AutoCAD_server\ShareSettings\UsersEditor\UsersEditor.lock";
#else
                var file = Path.GetSharedCommonFile("UsersEditor", "UsersEditor.lock");
#endif
                fileLock = new FileLock(file);
                if (!fileLock.IsLockSuccess)
                {
                    ShowMessage(fileLock.GetMessage(), "Занято, редактирует:");
                    EditMode = false;
                }
                else
                {
                    // Обновить данные
                    LoadUsers();
                    Users.Refresh();
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
                return;
            }
            IsOneUserSelected = SelectedUsers.Count == 1;
            SelectedUser = new EditAutocadUsers
            {
                FIO = GetValue(u => u.FIO),
                Login = GetValue(u => u.Login),
                Group = GetValue(u => u.Group),
                Disabled = GetValue(u => u.Disabled),
                Description = GetValue(u => u.Description),
            };
            var canApply = canEdit.CombineLatest(SelectedUser.Changed.Select(s => true), (b1,b2)=> b1 && b2);
            Apply = CreateCommand(() => ApplyExecute(SelectedUser, SelectedUsers), canApply);
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

        private void DeleteUserExec(EditAutocadUsers user)
        {
            dbUsers.DeleteUser(user.DbUser);
            users.Remove(user);
            Users.Refresh();
        }

        private T GetValue<T>(Func<EditAutocadUsers, T> prop)
        {
            if (SelectedUsers?.Any() == false) return default;
            var res = SelectedUsers.GroupBy(prop).Select(s => s.Key);
            var moreOne = res.Skip(1).Any();
            var value = res.First();
            return moreOne ? default : value;
        }
    }
}