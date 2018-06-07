using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AcadLib.User.UsersEditor;
using NetLib;
using NetLib.AD;
using NLog;
namespace UtilsEditUsers
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static ILogger Log { get; } = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                var usersVM = new UsersEditorVM();
                var usersView = new UsersEditorView(usersVM);
                usersView.Show();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}