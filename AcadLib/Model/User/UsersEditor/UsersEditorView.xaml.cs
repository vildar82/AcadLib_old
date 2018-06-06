using System.Linq;
using System.Windows.Controls;

namespace AcadLib.User.UsersEditor
{
    /// <summary>
    /// Interaction logic for UsersEditorView.xaml
    /// </summary>
    public partial class UsersEditorView
    {
        public UsersEditorView(UsersEditorVM vm) : base(vm)
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sel = dgUsers.SelectedItems.Cast<EditAutocadUsers>().ToList();
            ((UsersEditorVM) Model).SelectedUsers = sel;
        }
    }
}
