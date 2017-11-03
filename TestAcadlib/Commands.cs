using System.Collections.Generic;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using AcadLib.Jigs;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace TestAcadlib
{
    public class Commands
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
    }
}
