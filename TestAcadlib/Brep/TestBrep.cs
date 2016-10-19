using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace TestAcadlib.Brep
{
    public class TestBrep
    {
        [CommandMethod("TestBrepUnion")]
        public void TestBrepUnion ()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            var selPr = new PromptSelectionOptions();            
            ed.GetSelection();
            
        }
    }
}
