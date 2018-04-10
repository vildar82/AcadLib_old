using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Styles.StyleManager.UI;

namespace AcadLib.Styles.StyleManager
{
    public static class StyleManagerService
    {
        public static void ManageStyles()
        {
            var smVM = new StyleManagerVM();
            var smView = new StyleManagerView(smVM);
            smView.Show();
        }
    }
}
