namespace AcadLib.User.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoCAD_PIK_Manager.Settings;
    using AutoCAD_PIK_Manager.User;
    using NetLib;
    using NetLib.WPF;
    using ReactiveUI;

    public class UserSettingsVM : BaseViewModel
    {
        public UserSettingsVM(AutocadUser user)
        {
            User = user;
            Groups = LoadGroups();
            if (user != null && !user.Group.IsNullOrEmpty())
            {
                FillGroup(user.Group);
            }
            else if (!PikSettings.UserGroup.IsNullOrEmpty())
            {
                FillGroup(PikSettings.UserGroup);
            }
            else
            {
                Group = Groups[0];
            }

            Disabled = user?.Disabled ?? false;
            Ok = CreateCommand(OkExec);
            DeleteExtraGroup = CreateCommand(() => ExtraGroup = null);
            this.WhenAnyValue(v => v.Group).Subscribe(s => UpdateExtraGroups());
        }

        public AutocadUser User { get; set; }

        public List<UserGroup> Groups { get; set; }

        public UserGroup Group { get; set; }

        public List<UserGroup> ExtraGroups { get; set; }

        public UserGroup ExtraGroup { get; set; }

        public bool Disabled { get; set; }

        public ReactiveCommand Ok { get; set; }

        public ReactiveCommand DeleteExtraGroup { get; set; }

        private static UserGroup FindGroup(List<UserGroup> groups, string name)
        {
            return groups.FirstOrDefault(g => g.Name == name);
        }

        private List<UserGroup> LoadGroups()
        {
            var fileGroups = Path.Combine(PikSettings.ServerSettingsFolder, @"Общие\Dll\groups.json");
            var groups = fileGroups.Deserialize<Dictionary<string, string>>();
            return groups.Select(s => new UserGroup
            {
                Name = s.Key,
                Description = s.Value
            }).OrderBy(o => o.Name).ToList();
        }

        private void FillGroup(string userGroup)
        {
            var groups = userGroup.Split(',');
            var group = groups[0].Trim();
            Group = FindGroup(Groups, group);
            if (Group == null || groups.Length <= 1)
                return;
            var extraGroup = groups[1].Trim();
            if (extraGroup != group)
            {
                ExtraGroup = FindGroup(Groups, extraGroup);
            }
        }

        private void UpdateExtraGroups()
        {
            var groups = Groups.ToList();
            if (Group != null)
                groups.Remove(Group);
            ExtraGroups = groups;
        }

        private void OkExec()
        {
            if (User == null)
            {
                User = new AutocadUser
                {
                    FIO = UserInfo.FioAD,
                    Login = Environment.UserName.ToLower()
                };
            }

            User.Group = Group.Name;
            User.Disabled = Disabled;
            if (ExtraGroup != null)
            {
                User.Group += $", {ExtraGroup.Name}";
            }

            DialogResult = true;
        }
    }
}