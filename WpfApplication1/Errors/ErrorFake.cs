using AcadLib.Errors;
using System.Drawing;

namespace WpfApplication1.Errors
{
    public class ErrorFake : Error
    {
        public ErrorFake(string msg, Icon icon)
        {
            _msg = PrepareMessage(msg);
            _shortMsg = GetShortMsg(_msg);            
            Icon = icon;            
            DefineStatus();
            HasEntity = true;
            CanShow = HasEntity;
        }        

        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }        

        public override void Show()
        {
            //MessageBox.Show($"Show - {Message}");
        }
    }
}
