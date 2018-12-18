using BPNet.Module;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BPNet
{
	public class MatrixData
	{
		private List<Character> Characters = new List<Character>();
		private JavaScriptSerializer JSON = new JavaScriptSerializer();

		public void Clear()
		{
			Characters.Clear();
		}

		/// <summary>
		/// 采集一组笔画(这里面把每个笔画分为单独的字)
		/// </summary>
		/// <param name="strokes"></param>
		public void RecordOne(Point[][] strokes)
		{
			foreach (var item in strokes)
			{
				var stroke = new Stroke(item);
				Characters.Add(new Character() { Strokes = new List<Stroke>() { stroke } });
			}
		}

		/// <summary>
		/// 加载数据
		/// </summary>
		/// <param name="path"></param>
		public void LoadData(string path)
		{
			if (!File.Exists(path))
			{
				Characters = new List<Character>();
				return;
			}
			Characters = JSON.Deserialize<List<Character>>(File.ReadAllText(path));
		}

		/// <summary>
		/// 保存数据
		/// </summary>
		/// <param name="path"></param>
		public void SaveData(string path)
		{
			File.WriteAllText(path, JSON.Serialize(Characters));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Character[] ToArray()
		{
			return Characters.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="matrix_size"></param>
		/// <param name="steps"></param>
		/// <returns></returns>
		private int[][,] ToMatrix(Size matrix_size, out List<int> steps)
		{
			var metrix = new List<int[,]>();
			steps = new List<int>();
			for (int id_char = 0; id_char < Characters.Count; id_char++)
			{
				var character = Characters[id_char];

				//每个图分成多个笔画,一笔一笔画到矩阵上
				metrix.Add(new int[matrix_size.Width, matrix_size.Height]);

				int step = 1;

				foreach (var stroke in character.Strokes)
				{
					var side = stroke.Points.ToArray().FindSide();
					//内容 与 矩阵 比例  这里是等比缩放,所以横竖轴线只取短轴的比例即可
					double scale = 0;
					//字符区域的大小
					var charzone_size = new Size(side.Right - side.Left, side.Bottom - side.Top);
					double scale_x = (double)(matrix_size.Width - 1) / (double)charzone_size.Width;
					double scale_y = (double)(matrix_size.Height - 1) / (double)charzone_size.Height;
					scale = scale_x < scale_y ? scale_x : scale_y;

					//每个笔画分成多个像素点
					for (int p = 0; p < stroke.Points.Count(); p++)
					{
						stroke.Points[p].X = (int)((stroke.Points[p].X - side.Left) * scale);
						stroke.Points[p].Y = (int)((stroke.Points[p].Y - side.Top) * scale);
						if (p == 0) continue;//第一笔属于起点,跳过
						var distance = Extends.Distance(stroke.Points[p - 1], stroke.Points[p]);
						if (distance <= 0) continue;
						var ratio = (double)(stroke.Points[p].X - stroke.Points[p - 1].X) / (double)distance;

						var angle = ratio.Acos();

						for (int stepofstroke = 0; stepofstroke < distance; stepofstroke++)
						{
							int x = (int)(angle.Cos() * stepofstroke + stroke.Points[p - 1].X);
							int y = (int)(angle.Sin() * stepofstroke + stroke.Points[p - 1].Y);
							metrix[id_char][x, y] = step;
							step += 1;
						}
					}
					steps.Add(step);

				}

			}
			return metrix.ToArray();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="matrix_size"></param>
		/// <returns></returns>
		public int[][,] ToIntMatrix(Size matrix_size)
		{
			return ToMatrix(matrix_size, out var steps);
		}

		/// <summary>
		/// 
		/// </summary>
		public double[][,] ToDoubleMatrix(Size matrix_size)
		{
			var metrix = new double[Characters.Count][,];
			var intmetrix = ToMatrix(matrix_size, out var steps);

			for (int i = 0; i < metrix.Length; i++)
			{
				metrix[i] = new double[matrix_size.Width, matrix_size.Height];
				double floatsteps = 1.0d / (double)(steps[i] - 1);
				for (int x = 0; x < matrix_size.Width; x++)
				{
					for (int y = 0; y < matrix_size.Height; y++)
					{
						metrix[i][x, y] = intmetrix[i][x, y] * floatsteps;
					}
				}
			}
			return metrix;
		}



	}//End Class
}
