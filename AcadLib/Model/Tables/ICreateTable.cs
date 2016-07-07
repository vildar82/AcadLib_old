using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Tables
{
    public interface ICreateTable
    {
        string Layer { get; set; }
        int NumColumns { get; set; }
        int NumRows { get; set; }
        string Title { get; set; }

        void CalcRows ();
        Table Create ();
        void Insert (Table table, Document doc);
    }
}