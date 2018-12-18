using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPNet.Module
{
	/// <summary>
	/// 
	/// </summary>
	public class TrainData
	{
		/// <summary>
		/// 矩阵数据
		/// </summary>
		public double[,] MatrixData = null;
		/// <summary>
		/// 答案数据
		/// </summary>
		public double[,] Anwser = null;

		/// <summary>
		/// 
		/// </summary>
		public TrainData()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <param name="id"></param>
		public TrainData(string file, int id)
		{
			MatrixData = Extends.GetDim1Matrix(file);
			Anwser = new double[MatrixData.GetLength(0), 7];
			for (int i = 0; i < Anwser.GetLength(0); i++) Anwser[i, id] = 1;
		}
	}

}
