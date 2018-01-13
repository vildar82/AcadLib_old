using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AcadLib.XData
{
    [PublicAPI]
    public interface IDboDataSave : IExtDataSave, ITypedDataValues
    {
        string PluginName { get; set; }

        DBObject GetDBObject();
    }
}