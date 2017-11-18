using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Geometry.Polylines.Join
{
    class JoinPolyline
    {
        public Point3d Pt { get; set; }
        public Polyline Pl { get; set; }
        public bool IsStartPt { get; set; }
        public JoinPolyline OtherEndJoinPolyline { get; set; }
        public bool IsActualPt { get; set; } = true;
    }
}