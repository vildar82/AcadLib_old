using AcadLib.Errors;
using System.Collections.Generic;

namespace ConsoleApplication1.Errors
{
    public static class TestErrors
    {
        public static void TestShowErrors ()
        {
            var errors = new List<IError>();
            for (var i = 0; i < 10; i++)
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
