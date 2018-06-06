using System;
using System.Collections.Generic;
using System.IO;
using ConsoleApplication1.DbAutocadUsersTableAdapters;
using OfficeOpenXml;

namespace ConsoleApplication1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CheckUsers();
            //var users = GetUsersFromExcel();
            //SaveUsers(users);
            Console.ReadKey();
        }

        private static void CheckUsers()
        {
            using (var db = new AutocadUsersTableAdapter())
            {
                var dictUsers = new Dictionary<string, string>();
                var users = db.GetData();
                foreach (var user in users)
                {
                    var login = user.Login.ToLower();
                    if (dictUsers.ContainsKey(login))
                    {
                        Console.WriteLine(login);
                        continue;
                    }
                    dictUsers.Add(login, user.Group);
                }
            }
        }

        private static void SaveUsers(List<User> users)
        {
            using (var db = new AutocadUsersTableAdapter())
            {
                var table = db.GetData();
                foreach (var user in users)
                {
                    table.AddAutocadUsersRow(user.Login, user.FIO, user.Group, user.Disabled, user.Description);
                }
                db.Update(table);
            }
        }

        private static List<User> GetUsersFromExcel()
        {
            var users = new List<User>();
            var usersFile = @"\\picompany.ru\pikp\lib\_CadSettings\AutoCAD_server\Users\UserList2.xlsx";
            using (var excel = new ExcelPackage(new FileInfo(usersFile)))
            {
                var ws = excel.Workbook.Worksheets["Sheet1"];
                var r = 2;
                while (true)
                {
                    var fio = ws.Cells[r, 1].Text.Trim();
                    var login = ws.Cells[r, 2].Text.Trim();
                    var group = ws.Cells[r, 3].Text.Trim();
                    var desc = ws.Cells[r, 4].Text.Trim();
                    var disabled = ws.Cells[r, 5].Text.Trim();

                    if (string.IsNullOrEmpty(login)) break;
                    var user = new User
                    {
                        Login = login,
                        FIO = fio,
                        Group = group,
                        Description = desc,
                        Disabled = disabled == "1"
                    };
                    users.Add(user);
                    r++;
                }
            }

            return users;
        }
    }

    internal class User
    {
        public string Login { get; set; }
        public string FIO { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public bool Disabled { get; set; }
    }
}