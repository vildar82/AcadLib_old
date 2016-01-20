using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
   public partial class Form1 : Form
   {
      public Form1()
      {
         InitializeComponent();
      }

      private void button1_Click(object sender, EventArgs e)
      {
         AcadLib.UI.FileFolderDialog ffd = new AcadLib.UI.FileFolderDialog();         
         ffd.IsFolderDialog = true;
         ffd.Dialog.Multiselect = true;
         ffd.Dialog.Filter = "Чертежи|*.dwg";
         var res = ffd.ShowDialog();
         if (res == DialogResult.OK)
         {
            var selPath = ffd.SelectedPath;
            var selFiles = ffd.Dialog.FileNames;            
         }
      }
   }
}
