using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Geometry
{
    public class PolylineVertex
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public Point2d Pt { get; set; }

        public PolylineVertex (string name, int index, Point2d pt)
        {
            Name = name;
            Index = index;
            Pt = pt;
        }

        public static List<PolylineVertex> GetVertexes (Polyline pl, string name)
        {
            var res = new List<PolylineVertex>();
            for (var i = 0; i < pl.NumberOfVertices; i++)
            {
                var pt = pl.GetPoint2dAt(i);
                res.Add(new PolylineVertex(name, i, pt));
            }
            return res;
        }
    }
}
