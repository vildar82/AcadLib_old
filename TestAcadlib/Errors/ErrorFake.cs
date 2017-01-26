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
