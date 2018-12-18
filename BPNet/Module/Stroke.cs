using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPNet.Module
{
	/// <summary>
	/// 
	/// </summary>
	public class Stroke
	{
		public Stroke()
		{
		}
		public Stroke(Point[] ps)
		{
			Points.AddRange(from p in ps select new ClsPoint(p));
		}
		/// <summary>
		/// All point of the stroke
		/// </summary>
		public List<ClsPoint> Points = new List<ClsPoint>();
	}
}
