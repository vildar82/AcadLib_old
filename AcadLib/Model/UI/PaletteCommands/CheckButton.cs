using System;
using System.Drawing;
using System.Windows.Input;
using AcadLib.PaletteCommands;
using NetLib.WPF.Data;

namespace AcadLib.UI.PaletteCommands
{
    public class CheckButton : PaletteCommand
    {
        public CheckButton(string name, Bitmap icon, bool isChecked, Func<bool> change, string desc, string group)
        {
            Change = change;
            Name = name;
            Image = GetSource(icon, false);
            Group = group;
            Description = desc;
            IsChecked = isChecked;
        }

        public Func<bool> Change { get; }

        public bool IsChecked { get; set; }

        public override void Execute()
        {
            IsChecked = Change();
        }
    }
}