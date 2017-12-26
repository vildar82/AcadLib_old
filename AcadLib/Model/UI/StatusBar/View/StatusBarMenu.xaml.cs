using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Autodesk.AutoCAD.ApplicationServices;
using JetBrains.Annotations;

namespace AcadLib.UI.StatusBar.View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class StatusBarMenu
    {
        public List<MenuItem> MenuItems { get; set; }

        public StatusBarMenu(string value, [NotNull] List<string> values, Action<string> selectValue)
        {
            Left = System.Windows.Forms.Cursor.Position.X;
            Top = System.Windows.Forms.Cursor.Position.Y;
            var menuItems = values
                .Select(s =>
                {
                    var mi = new MenuItem
                    {
                        Header = s,
                        IsChecked = Equals(s, value)
                    };
                    mi.Click += (oc, ec) =>
                    {
                        selectValue(s);
                        Hide();
                    };
                    return mi;
                }).ToList();
            MenuItems = menuItems;
            InitializeComponent();
            DataContext = this;
            Activated += (o, e) =>
            {
                Left -= ActualWidth;
                Top -= ActualHeight;
            };
            Deactivated += (o, e) => Close();
        }
    }
}
