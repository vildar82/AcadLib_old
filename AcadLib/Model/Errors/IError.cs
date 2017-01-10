using System.Drawing;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace AcadLib.Errors
{
    public interface IError : IComparable<IError>, IEquatable<IError>
    {
        Extents3d Extents { get; }
        bool HasEntity { get; }
        bool CanShow { get; }
        Icon Icon { get; set; }
        ObjectId IdEnt { get; }
        string Message { get; }
        string ShortMsg { get; }
        object Tag { get; set; }
        Matrix3d Trans { get; set; }
        void AdditionToMessage(string addMsg);        
        IError GetCopy();        
        void Show();        
    }
}