using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace AcadLib.PaletteProps
{
    /// <summary>
    /// Interaction logic for IntValueView.xaml
    /// </summary>
    public partial class IntValueView
    {
        public IntValueView(IntValueVM vm) : base(vm, false)
        {
            InitializeComponent();
        }

        private void UIElement_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is NumericUpDown numUpDown)
            {
                var bindExpr = BindingOperations.GetBindingExpression(numUpDown, NumericUpDown.ValueProperty);
                bindExpr?.UpdateSource();
            }
        }
    }
}
