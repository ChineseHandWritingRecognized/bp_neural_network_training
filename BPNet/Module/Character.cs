using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPNet.Module
{
	/// <summary>
	/// A Chinese character or a strokes package
	/// </summary>
	public class Character
	{
		/// <summary>
		/// All strokes of the character
		/// </summary>
		public List<Stroke> Strokes = new List<Stroke>();
	}
}
