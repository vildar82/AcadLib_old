using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.XData
{
    public interface IDboDataSave : IExtDataSave, ITypedDataValues
    {
        string PluginName { get; set; }

        DBObject GetDBObject ();
    }
}
