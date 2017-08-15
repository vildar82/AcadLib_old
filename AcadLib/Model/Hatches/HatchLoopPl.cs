using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Hatches
{
	public class HatchLoopPl : IDisposable
	{
		public Curve Loop { get; set; }
		public HatchLoopTypes Types { get; set; }

		public void Dispose()
		{
			Loop?.Dispose();
		}

		public Polyline GetPolyline()
		{
			switch (Loop)
			{
				case Polyline pl: return pl;
			}
			return null;
		}
	}
}
