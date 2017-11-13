using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib.UI.Properties
{
    public static class PropertiesService
    {
        public static bool? Show(object value, Func<object, object> reset = null)
        {
            var propVM = new PropertiesViewModel(value, reset);
            var propView = new PropertiesView(propVM);
            return propView.ShowDialog();
        }
    }
}
