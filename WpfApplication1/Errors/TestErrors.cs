using AcadLib.Errors;
using System.Collections.Generic;
using System.Linq;

namespace WpfApplication1.Errors
{
    public static class TestErrors
    {
        public static void TestShowErrors()
        {
            var errors = new List<IError>
            {
                new ErrorFake(
                    "Наложение зданий. Площадь наложения 335.8. 'Жилое H=47.8 Проектируемое б/с СЭМ2 36 ' и 'Жилое H=47.8 Проектируемое б/с СЭМ2 40 '.",
                    System.Drawing.SystemIcons.Error),
                new ErrorFake(
                    "Наложение зданий. Площадь наложения 335.8. 'Жилое H=47.8 Проектируемое б/с СЭМ2 36 ' и 'Жилое H=47.8 Проектируемое б/с СЭМ2 40 '.",
                    System.Drawing.SystemIcons.Error),
                new ErrorFake(
                    "Наложение зданий. Площадь наложения 335.8. 'Жилое H=47.8 Проектируемое б/с СЭМ2 36 ' и 'Жилое H=47.8 Проектируемое б/с СЭМ2 40 '.",
                    System.Drawing.SystemIcons.Information)
            };
            for (var i = 0; i < 100; i++)
            {
                errors.Add(new ErrorFake("Сообщение об ошибке", System.Drawing.SystemIcons.Exclamation));
                errors.Add(new ErrorFake(string.Concat(Enumerable.Repeat("Сообщение об ошибке", i)), System.Drawing.SystemIcons.Exclamation));
            }
            var errVM = new ErrorsViewModel(errors)
            {
                IsDialog = true
            };
            var errView = new ErrorsView(errVM);
            errView.Show();
        }
    }
}
