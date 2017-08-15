using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AcadLib.UI.Ribbon.Elements
{
	public interface IRibbonElement
	{
		string Tab { get; set; }
		string Panel { get; set; }
		string CommandName { get; set; }
		string Title { get; set; }
		ImageSource LargeImage { get; set; }
		ImageSource Image { get; set; }
	}
}
