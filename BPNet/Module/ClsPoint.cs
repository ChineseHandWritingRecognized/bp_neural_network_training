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
	public class ClsPoint
	{
		public ClsPoint()
		{
		}
		public ClsPoint(Point p)
		{
			X = p.X;
			Y = p.Y;
		}
		/// <summary>
		/// 
		/// </summary>
		public double X;
		/// <summary>
		/// 
		/// </summary>
		public double Y;
	}
}
