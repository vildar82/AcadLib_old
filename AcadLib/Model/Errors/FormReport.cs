using System;
using System.Windows.Forms;

namespace AcadLib.Errors
{
    public partial class FormReport : Form
    {
        public string Message { get; set; }

        public FormReport ()
        {
            InitializeComponent();
        }

        private void button1_Click (object sender, EventArgs e)
        {
            Message = richTextBox1.Text;
        }
    }
}
