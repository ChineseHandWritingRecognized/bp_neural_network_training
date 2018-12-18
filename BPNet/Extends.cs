using BPNet.Module;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPNet
{
	/// <summary>
	/// 
	/// </summary>
	public static class Extends
	{
		/// <summary>
		/// 获取二维矩阵数组
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static double[][,] GetMatrix(string file)
		{
			var traindata = new MatrixData();
			traindata.LoadData(file);
			return traindata.ToDoubleMatrix(new Size(16, 16));
		}

		/// <summary>
		/// 获取一维矩阵(由于神经网络代码必须按照这个格式输入..)
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static double[,] GetDim1Matrix(string file)
		{
			var matrix = GetMatrix(file);
			var dim1matrix = new double[matrix.Length, matrix[0].Length];
			for (int i = 0; i < dim1matrix.GetLength(0); i++)
			{
				var buff = new List<double>();
				int point = 0;
				for (int x = 0; x < matrix[i].GetLength(0); x++)
				{
					for (int y = 0; y < matrix[i].GetLength(1); y++)
					{
						dim1matrix[i, point] = matrix[i][x, y];
						point++;
					}
				}
			}
			return dim1matrix;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static UInt16 LowWord(this UInt32 u32)
		{
			return (UInt16)u32;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static UInt16 HighWord(this UInt32 u32)
		{
			return (UInt16)(u32 >> 16);
		}

		/// <summary>
		/// 寻边
		/// </summary>
		/// <param name="strokes"></param>
		/// <returns></returns>
		internal static Side FindSide(this ClsPoint[] strokes)
		{
			var side = new Side();
			if (strokes == null) return null;
			if (strokes.Length <= 0) return null;
			side.Left = int.MaxValue;
			side.Top = int.MaxValue;
			side.Bottom = int.MinValue;
			side.Right = int.MinValue;
			foreach (var point in strokes)
			{
				var x = point.X;
				var y = point.Y;
				if (x == 0xFFFF) continue;
				if (y == 0xFFFF) break;
				if (x < side.Left) side.Left = (int)x;
				if (x > side.Right) side.Right = (int)x;
				if (y < side.Top) side.Top = (int)y;
				if (y > side.Bottom) side.Bottom = (int)y;
			}
			return side;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="arr"></param>
		/// <param name="another_column_index"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public static T[] GetColumn<T>(this T[,] arr, int another_column_index, int column = 1)
		{
			var subarr = new T[arr.GetLength(column)];
			for (int i = 0; i < subarr.Length; i++)
			{
				if (column == 0) subarr[i] = arr[i, another_column_index];
				if (column == 1) subarr[i] = arr[another_column_index, i];
			}
			return subarr;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public static double Distance(Point p1, Point p2)
		{
			var distant_x = Math.Pow(p2.X - p1.X, 2);//两点在横轴上的距离
			var distant_y = Math.Pow(p2.Y - p1.Y, 2);//两点在竖轴上的距离

			return Math.Sqrt(distant_x + distant_y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public static double Distance(ClsPoint p1, ClsPoint p2)
		{
			var distant_x = Math.Pow(p2.X - p1.X, 2);//两点在横轴上的距离
			var distant_y = Math.Pow(p2.Y - p1.Y, 2);//两点在竖轴上的距离

			return Math.Sqrt(distant_x + distant_y);
		}

		/// <summary>
		/// 按角度计算的Sin
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static double Sin(this double d)
		{
			var ret = Math.Sin((d * Math.PI) / 180);
			return ret;
		}

		/// <summary>
		/// 按角度计算的Cos
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static double Cos(this double d)
		{
			return Math.Cos((d * Math.PI) / 180);
		}

		/// <summary>
		/// 按角度计算的Acos
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static double Acos(this double d)
		{
			return Math.Acos(d) * 180 / Math.PI;
		}


	}

}
