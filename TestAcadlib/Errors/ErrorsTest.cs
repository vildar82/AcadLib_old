using AcadLib.Errors;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAcadlib.Errors
{
    public static class ErrorsTest
    {
        [CommandMethod("TestShowErrorsDialog")]
        public static void TestShowErrorsDialog()
        {
            for (int i = 0; i < 10; i++)
            {
                Inspector.AddError("Сообщение об ошибке", System.Drawing.SystemIcons.Error);
                Inspector.AddError("Сообщение об ошибке" + i, System.Drawing.SystemIcons.Error);
            }
            Inspector.ShowDialog();
        }
    }
}
