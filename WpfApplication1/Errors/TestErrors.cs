using AcadLib.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1.Errors
{
    public static class TestErrors
    {
        public static void TestShowErrors ()
        {
            var errors = new List<IError>();
            errors.Add(new ErrorFake("Наложение зданий. Площадь наложения 335.8. 'Жилое H=47.8 Проектируемое б/с СЭМ2 36 ' и 'Жилое H=47.8 Проектируемое б/с СЭМ2 40 '."));
            errors.Add(new ErrorFake("Наложение зданий. Площадь наложения 335.8. 'Жилое H=47.8 Проектируемое б/с СЭМ2 36 ' и 'Жилое H=47.8 Проектируемое б/с СЭМ2 40 '."));
            errors.Add(new ErrorFake("Наложение зданий. Площадь наложения 335.8. 'Жилое H=47.8 Проектируемое б/с СЭМ2 36 ' и 'Жилое H=47.8 Проектируемое б/с СЭМ2 40 '."));
            for (int i = 0; i < 10; i++)
            {
                errors.Add(new ErrorFake("Сообщение об ошибке"));
                errors.Add(new ErrorFake( string.Concat(Enumerable.Repeat("Сообщение об ошибке", i))));              
            }
            var errVM = new ErrorsViewModel(errors);
            var errView = new ErrorsView(errVM);            
            errView.Show();
        }
    }
}
