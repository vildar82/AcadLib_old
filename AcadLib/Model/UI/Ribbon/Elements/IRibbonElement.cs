using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace AcadLib.UI.Ribbon.Elements
{
	public interface IRibbonElement
	{
		string Tab { get; set; }
		string Panel { get; set; }
		ICommand Command { get; set; }
		string Name { get; set; }
		ImageSource LargeImage { get; set; }
		ImageSource Image { get; set; }
        string Description { get; set; }
    }
}
