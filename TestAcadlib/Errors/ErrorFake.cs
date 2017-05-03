using AcadLib.Errors;
using System.Drawing;
using System.Windows;

namespace TestAcadlib.Errors
{
    public class ErrorFake : Error
    {
        public ErrorFake(string msg)
        {
            _msg = msg;
            _shortMsg = msg;
            Icon = SystemIcons.Error;
            _hasEntity = true;
        }

        
        public void Show()
        {
            MessageBox.Show($"Show - {Message}");
        }
    }
}
