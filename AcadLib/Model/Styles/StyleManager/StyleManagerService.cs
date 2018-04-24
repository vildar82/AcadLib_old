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
