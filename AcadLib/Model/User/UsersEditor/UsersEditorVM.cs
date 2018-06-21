#if Utils
using UtilsEditUsers.Model.User.DB;
#else
using Path = AcadLib.IO.Path;
using AutoCAD_PIK_Manager.Settings;
using AcadLib.Model.User.DB;
#endif

namespace AcadLib.User.UsersEditor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using DB;
    using NetLib;
    using NetLib.AD;
    using NetLib.Locks;
    using NetLib.Monad;
    using NetLib.WPF;
    using NetLib.WPF.Data;
    using ReactiveUI;
    using UI;
    using General = AcadLib.General;
    using Path = IO.Path;

    public class UsersEditorVM : BaseViewModel
    {
        private static Brush colorOk = new SolidColorBrush(System.Windows.Media.Colors.MediumSeaGreen);
        private static Brush colorErr = new SolidColorBrush(System.Windows.Media.Colors.IndianRed);
#if Utils
        const string serverSettingsDir = @"\\picompany.ru\pikp\lib\_CadSettings\AutoCAD_server\Адаптация";
        const string serverShareDir = @"\\picompany.ru\pikp\lib\_CadSettings\AutoCAD_server\ShareSettings";
#endif
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
            imageNo = new BitmapImage(new Uri("pack://application:,,,/Resources/no-user.png"));
            var groups = ADUtils.GetCurrentUserADGroups(out _);
            IsBimUser = groups.Any(g => g.EqualsIgnoreCase("010583_Отдел разработки и автоматизации") ||
                                    g.EqualsIgnoreCase("010596_Отдел внедрения ВIM") ||
                                    g.EqualsIgnoreCase("010576_УИТ"));
#else
            IsBimUser = General.IsBimUser;
#endif

            dbUsers = new DbUsers();
            this.WhenAnyValue(v => v.EditMode).Subscribe(ChangeMode);
            this.WhenAnyValue(v => v.SelectedUsers).Subscribe(s => OnSelected());
            this.WhenAnyValue(v => v.Filter, v => v.FilterGroup).Skip(1).Subscribe(s =>
            {
                var serverVer = GroupServerVersions.FirstOrDefault(g => g.Name == s.Item2);
                if (serverVer != null)
                    GroupServerVersion = serverVer;
                Users.Refresh();
            });
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

        public List<UserGroup> GroupServerVersions { get; set; }

        public UserGroup GroupServerVersion { get; set; }

        public int UsersCount { get; set; }

        private async void LoadUsers()
        {
            GroupServerVersions = await LoadGroupServerVersionsAsync();
            users = dbUsers.GetUsers().Select(GetUser).ToList();
            Users = new CollectionView<EditAutocadUsers>(users) { Filter = OnFilter };
            Users.CollectionChanged += (o, e) => UsersCount = Users.Count();
#if Utils
            Groups = LoadUserGroups();
#else
            Groups = PikSettings.UserGroups;
#endif
            LoadUsersEx();
            FilterGroups = users.SelectMany(s => GetGroups(s.Group)).GroupBy(g => g).Select(s => s.Key).OrderBy(o => o).ToList();
            FilterGroups.Insert(0, "Все");
        }

        private EditAutocadUsers GetUser(AutocadUsers userDb)
        {
            var (brush, tooltip) = GetUserVerionInfo(userDb);
            return new EditAutocadUsers(userDb)
            {
                VersionColor = brush,
                VersionTooltip = tooltip
            };
        }

        private (Brush color, string tooltip) GetUserVerionInfo(AutocadUsers userDb)
        {
            if (userDb.Group.IsNullOrEmpty() || userDb.Version.IsNullOrEmpty())
                return (null, null);
            Brush color = null;
            var tooltip = string.Empty;
            var userGroups = GetGroups(userDb.Group).ToList();
            userGroups.Add("Общие");
            var isOk = false;
            foreach (var @group in userGroups)
            {
                var serGroup = GroupServerVersions.FirstOrDefault(f => f.Name == group);
                if (serGroup == null)
                    continue;
                var verSer = serGroup.Version;
                var verMatch = Regex.Match(userDb.Version, $@"{@group}=(\d+)");
                if (verMatch.Success)
                {
                    isOk = true;
                    var verLoc = verMatch.Groups[1].Value;
                    if (verLoc != verSer)
                    {
                        color = colorErr;
                        tooltip += $"{group} - на сервере {verSer}\n";
                    }
                }
            }

            if (isOk && color == null)
            {
                color = colorOk;
            }

            tooltip = tooltip.Trim();
            return (color, tooltip);
        }

        private Task<List<UserGroup>> LoadGroupServerVersionsAsync()
        {
            return Task.Run(() =>
            {
                var groups = new List<UserGroup>();
#if Utils
                var dirGroups = serverSettingsDir;
#else
                var dirGroups = PikSettings.ServerSettingsFolder;
#endif

                foreach (var dirGroup in Directory.EnumerateDirectories(dirGroups).OrderBy(o => o))
                {
                    var groupName = System.IO.Path.GetFileName(dirGroup);
                    if (string.IsNullOrEmpty(groupName))
                        continue;
                    var verFile = System.IO.Path.Combine(dirGroup, $"{groupName}.ver");
                    var ver = verFile.Try(f => File.ReadLines(f).FirstOrDefault());
                    if (ver.IsNullOrEmpty())
                        continue;
                    groups.Add(new UserGroup
                    {
                        Name = groupName,
                        Version = ver
                    });
                }
                return groups;
            });
        }

        private IEnumerable<string> GetGroups(string userGroup)
        {
            if (userGroup.Contains(','))
            {
                foreach (var s in userGroup.Split(',').Select(a => a.Trim()))
                    yield return s;
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
                        var dep = uData?.Department ?? "не определено";
                        var pos = uData?.Position ?? "не определено";
                        var image = u.Login.Try(l => UserSettingsService.LoadHomePikImage(l, "main")) ?? imageNo;
                        exUser = (dep, pos, image);
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
                if (!Filter.IsNullOrEmpty())
                    res = Regex.IsMatch(user.ToString(), Filter, RegexOptions.IgnoreCase);
                if (!res)
                    return false;
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

#if Utils
        private static List<string> LoadUserGroups()
        {
            var stringList = new List<string>();
            try
            {

                const string file = serverSettingsDir + @"\Общие\Dll\groups.json";
                stringList = file.Deserialize<Dictionary<string, string>>().Keys.ToList();
            }
            catch
            {
                //
            }

            return stringList;
        }
#endif

        private void ChangeMode(bool editMode)
        {
            if (editMode)
            {
                // Создать файл блокировки
#if Utils
                const string file = serverShareDir + @"\UsersEditor\UsersEditor.lock";
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
            var canApply = canEdit.CombineLatest(SelectedUser.Changed.Select(s => true), (b1, b2) => b1 && b2);
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
            if (SelectedUsers?.Any() == false)
                return default;
            var res = SelectedUsers.GroupBy(prop).Select(s => s.Key);
            var moreOne = res.Skip(1).Any();
            var value = res.First();
            return moreOne ? default : value;
        }
    }
}