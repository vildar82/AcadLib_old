using System;
using System.Windows.Forms;

namespace AcadLib.UI
{
    public partial class FormProperties : Form
    {
        public FormProperties()
        {
            InitializeComponent();                         
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            propertyGrid1.ResetSelectedProperty();            
        }
    }
}
