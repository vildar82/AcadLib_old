using System.Diagnostics;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(TestAcadlib.Commands))]

namespace TestAcadlib
{
    public class Commands : IExtensionApplication
    {
        //[CommandMethod("Test")]
        //public void Test()
        //{
        //    var doc = Application.DocumentManager.MdiActiveDocument;
        //    var ed = doc.Editor;
        //    var db = doc.Database;

        //    var tables = new List<Entity>();
        //    for (var i = 0; i < 10; i++)
        //    {
        //        var table = new Table();
        //        table.SetSize(i, i * 2);
        //        table.Cells[0, 0].TextString = i.ToString();
        //        table.GenerateLayout();
        //        tables.Add(table);                
        //    }           

        //    ed.Drag(tables, 50);
        //}       
        public void Initialize()
        {
#if DEBUG
            // Отключение отладочных сообщений биндинга (тормозит сильно)
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Off;
#endif
        }

        public void Terminate()
        {
            
        }
    }
}
