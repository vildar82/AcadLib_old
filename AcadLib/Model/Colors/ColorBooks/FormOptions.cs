using System.Windows.Forms;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AcadLib.Colors
{
    [PublicAPI]
    public partial class FormOptions : Form
    {
        public Options Options { get; set; }

        public FormOptions(Options options)
        {
            InitializeComponent();

            Options = options;
            propertyGrid1.SelectedObject = options;
        }
    }
}