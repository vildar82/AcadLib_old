using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Geometry
{
 	public static class CurveExt
	{
		public static List<Point3d> GetPoints(this Curve curve)
		{
			if (curve is Polyline pl) return pl.GetPoints().Select(s=>s.Convert3d()).ToList();
			var pts = new List<Point3d>();
			for (var i = curve.StartParam; i <= curve.EndParam; i+=0.1)
			{
				 var pt = curve.GetPointAtParameter(i);
				pts.Add(pt);
			}
			return pts;
		}
	}
}
