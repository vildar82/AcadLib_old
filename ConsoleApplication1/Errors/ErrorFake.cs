using AcadLib.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Drawing;
using System.Windows;

namespace ConsoleApplication1.Errors
{
    public class ErrorFake : IError
    {
        public ErrorFake(string msg)
        {
            Message = msg;
            ShortMsg = msg;
            Icon = SystemIcons.Error;
            HasEntity = true;
        }

        public Extents3d Extents { get; set; }

        public bool HasEntity { get; set; }
        public bool CanShow { get; set; }

        public Icon Icon { get; set; }

        public ObjectId IdEnt { get; set; }

        public string Message { get; set; }

        public string ShortMsg { get; set; }

        public object Tag { get; set; }

        public Matrix3d Trans { get; set; }

        public void AdditionToMessage(string addMsg)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IError other)
        {
            return this.Message.CompareTo(other.Message);
        }

        public bool Equals(IError other)
        {
            return this.Message.Equals(other.Message);
        }

        public IError GetCopy()
        {
            return (IError)MemberwiseClone();
        }

        public void Show()
        {
            MessageBox.Show($"Show - {Message}");
        }
    }
}
