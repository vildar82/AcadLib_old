using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;

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
