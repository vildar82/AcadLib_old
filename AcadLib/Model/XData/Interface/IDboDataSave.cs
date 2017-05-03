using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.XData
{
    public interface IDboDataSave : IExtDataSave, ITypedDataValues
    {
        string PluginName { get; set; }

        DBObject GetDBObject ();
    }
}
