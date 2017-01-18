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
            errors.Add(new ErrorFake("Наложение зданий. Площадь наложения 335.8. 'Жилое H=47.8 Проектируемое б/с СЭМ2 36 ' и 'Жилое H=47.8 Проектируемое б/с СЭМ2 40 '.", System.Drawing.SystemIcons.Error));
            errors.Add(new ErrorFake("Наложение зданий. Площадь наложения 335.8. 'Жилое H=47.8 Проектируемое б/с СЭМ2 36 ' и 'Жилое H=47.8 Проектируемое б/с СЭМ2 40 '.", System.Drawing.SystemIcons.Error));
            errors.Add(new ErrorFake("Наложение зданий. Площадь наложения 335.8. 'Жилое H=47.8 Проектируемое б/с СЭМ2 36 ' и 'Жилое H=47.8 Проектируемое б/с СЭМ2 40 '." ,System.Drawing.SystemIcons.Information));
            for (int i = 0; i < 100; i++)
            {
                errors.Add(new ErrorFake("Сообщение об ошибке", System.Drawing.SystemIcons.Exclamation));
                errors.Add(new ErrorFake( string.Concat(Enumerable.Repeat("Сообщение об ошибке", i)), System.Drawing.SystemIcons.Exclamation));              
            }            
            var errVM = new ErrorsViewModel(errors);
            errVM.IsDialog = true;
            var errView = new ErrorsView(errVM);            
            errView.Show();
        }
    }
}
