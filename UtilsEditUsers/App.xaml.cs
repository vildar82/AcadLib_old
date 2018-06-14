using System;
using System.Windows;
using AcadLib.User.UsersEditor;
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