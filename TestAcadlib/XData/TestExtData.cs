using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.XData;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace TestAcadlib.XData
{
    public class TestExtData
    {
        int deep = 0;

        [CommandMethod("TestSaveExtDataNOD")]
        public void TestSaveExtDataNOD ()
        {
            deep = 0;
            var nod = new DictNOD("Test", true);
            var rec = new DicED();
            rec.Name = "Test";

            rec.Recs = GetRecs("RecsTest", 5);
            rec.Inners = GetInners("InnersTest", 3);          

            nod.Save(rec);
        }

        [CommandMethod("TestLoadExtDataNOD")]
        public void TestLoadExtDataNOD()
        {
            var nod = new DictNOD("Test", true);
            var recEd = nod.LoadED("Test");
        }

        private List<RecXD> GetRecs (string name, int number)
        {
            var recs = new List<RecXD>();
            for (int i = 0; i < number; i++)
            {
                recs.Add(new RecXD { Name = name + i, Values = GetTestValues(name + i, number) });
            }
            return recs;
        }

        private List<DicED> GetInners (string name, int number)
        {                      
            var res = new List<DicED>();
            if (deep > 3) return res;
                deep++;

            for (int i = 0; i < number; i++)
            {
                var recEd = new DicED();
                recEd.Name = name + i;
                recEd.Recs = GetRecs("Recs_" + recEd.Name, 5);
                recEd.Inners = GetInners("Inners_" + recEd.Name, 2);

                res.Add(recEd);
            }            
            return res;
        }

        private List<TypedValue> GetTestValues (string name, int number)
        {
            var res = new List<TypedValue>();
            for (int i = 0; i < number; i++)
            {
                res.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, name + i));
            }
            return res;
        }
    }
}
