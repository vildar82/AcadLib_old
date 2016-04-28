using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
