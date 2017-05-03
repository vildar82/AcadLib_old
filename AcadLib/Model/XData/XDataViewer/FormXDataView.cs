using System.Windows.Forms;

namespace AcadLib.XData.Viewer
{
    public partial class FormXDataView : Form
    {
        public FormXDataView(string info, string entName)
        {
            InitializeComponent();
            label1.Text = entName;
            richTextBox1.Text = info;
        }        
    }
}
