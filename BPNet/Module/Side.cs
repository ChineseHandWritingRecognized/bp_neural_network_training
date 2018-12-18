using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPNet.Module
{
	/// <summary>
	/// Used to represent 4 side of a rectangle
	/// </summary>
	[Serializable]
	public class Side
	{
		[Browsable(true)]
		public int Left;

		[Browsable(true)]
		public int Top;
		[Browsable(true)]
		public int Right;

		[Browsable(true)]
		public int Bottom;

		public Side()
		{
		}

	}
}
