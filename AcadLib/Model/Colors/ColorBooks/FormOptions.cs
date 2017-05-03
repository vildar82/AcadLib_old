using System.Windows.Forms;

namespace AcadLib.Colors
{
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
