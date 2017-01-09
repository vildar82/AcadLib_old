using AcadLib.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConsoleApplication1.Errors
{
    public static class TestErrors
    {
        public static void TestShowErrors ()
        {
            var errors = new List<IError>();
            for (int i = 0; i < 10; i++)
            {
                errors.Add(new ErrorFake("Сообщение об ошибке"));
                errors.Add (new ErrorFake ("Сообщение об ошибке" + i));
            }
            var errVM = new ErrorsViewModel(errors);
            var errView = new ErrorsView(errVM);
            errView.Show();
        }
    }
}
