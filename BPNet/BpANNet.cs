using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPNet
{
	/// <summary>
	/// 
	/// </summary>
	internal class StringPack
	{
		public double[][] W = null;
		public double[][] V = null;
		public double[] B1 = null;
		public double[] B2 = null;
		public int iN = 0, hN = 0, oN = 0;
		public double iR = 0;
	}

	/// <summary>
	/// BpNet 的摘要说明。
	/// </summary>
	internal class BpNet
	{
		public BpNet(double[,] p, double[,] t)
		{
			// 构造函数逻辑
			R = new Random((int)DateTime.Now.Ticks);

			this.inNum = p.GetLength(1); //数组第二维大小为 输入节点数
			this.outNum = t.GetLength(1); //输出节点数
			this.hideNum = computeHideNum(inNum, outNum); //隐藏节点数，不知其原理

			//this.hideNum=18;
			var samplenum = p.GetLength(0); //数组第一维大小 为

			Console.WriteLine("输入节点数目： " + inNum);
			Console.WriteLine("隐层节点数目：" + hideNum);
			Console.WriteLine("输出层节点数目：" + outNum);

			x = new double[inNum];
			x1 = new double[hideNum];
			x2 = new double[outNum];

			o1 = new double[hideNum];
			o2 = new double[outNum];

			w = new double[inNum, hideNum];
			v = new double[hideNum, outNum];
			dw = new double[inNum, hideNum];
			dv = new double[hideNum, outNum];

			b1 = new double[hideNum];
			b2 = new double[outNum];
			db1 = new double[hideNum];
			db2 = new double[outNum];

			pp = new double[hideNum];
			qq = new double[outNum];
			yd = new double[outNum];

			//初始化w
			for (int i = 0; i < inNum; i++)
			{
				for (int j = 0; j < hideNum; j++)
				{
					w[i, j] = (R.NextDouble() * 2 - 1.0) / 2;
				}
			}

			//初始化v
			for (int i = 0; i < hideNum; i++)
			{
				for (int j = 0; j < outNum; j++)
				{
					v[i, j] = (R.NextDouble() * 2 - 1.0) / 2;
				}
			}

			rate = 0.8;
			e = 0.0;
			in_rate = 1.0;
		}

		public BpNet(string json) //仿真函数 用的构造函数
		{
			var stringpack = JSON.Deserialize<StringPack>(json);
			w = new double[stringpack.W.Length, stringpack.W[0].Length];
			v = new double[stringpack.V.Length, stringpack.V[0].Length];
			for (int i = 0; i < stringpack.W.Length; i++)
			{
				for (int j = 0; j < w.GetLength(1); j++) w[i, j] = stringpack.W[i][j];
			}
			for (int i = 0; i < stringpack.V.Length; i++)
			{
				for (int j = 0; j < v.GetLength(1); j++) v[i, j] = stringpack.V[i][j];
			}
			b1 = stringpack.B1;
			b2 = stringpack.B2;
			inNum = stringpack.iN;
			hideNum = stringpack.hN;
			outNum = stringpack.oN;
			in_rate = stringpack.iR;
		}

		public int inNum;//输入节点数
		int hideNum;//隐层节点数
		public int outNum;//输出层节点数

		//public int sampleNum;//样本总数

		Random R;
		private double[] x;//输入节点的输入数据
		private double[] x1;//隐层节点的输出
		private double[] x2;//输出节点的输出
		private double[] o1;//隐层的输入
		private double[] o2;//输出层的输入
		private double[,] w;//权值矩阵w
		private double[,] v;//权值矩阵V
		private double[,] dw;//权值矩阵w
		private double[,] dv;//权值矩阵V

		private double rate;//学习率
		private double[] b1;//隐层阈值矩阵
		private double[] b2;//输出层阈值矩阵
		private double[] db1;//隐层阈值矩阵
		private double[] db2;//输出层阈值矩阵

		double[] pp;//输出层的误差
		double[] qq;//隐层的误差
		double[] yd;//输出层的教师数据
		public double e;//均方误差
		double in_rate;//归一化比例系数

		private System.Web.Script.Serialization.JavaScriptSerializer JSON = new System.Web.Script.Serialization.JavaScriptSerializer();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public int computeHideNum(int m, int n)
		{
			double s = Math.Sqrt(0.43 * m * n + 0.12 * n * n + 2.54 * m + 0.77 * n + 0.35) + 0.51;
			int ss = Convert.ToInt32(s);
			return ((s - (double)ss) > 0.5) ? ss + 1 : ss;
		}

		/// <summary>
		/// 训练函数
		/// </summary>
		/// <param name="p"></param>
		/// <param name="t"></param>
		public void train(double[,] p, double[,] t)
		{
			var samplenum = p.GetLength(0);
			e = 0.0;
			//求p，t中的最大值
			double pMax = 0.0;
			for (int isamp = 0; isamp < samplenum; isamp++)
			{
				for (int i = 0; i < inNum; i++)
				{
					if (Math.Abs(p[isamp, i]) <= pMax) continue;
					pMax = Math.Abs(p[isamp, i]);
				}

				for (int j = 0; j < outNum; j++)
				{
					if (Math.Abs(t[isamp, j]) <= pMax) continue;
					pMax = Math.Abs(t[isamp, j]);
				}

				in_rate = pMax;
			}//end isamp



			for (int isamp = 0; isamp < samplenum; isamp++)
			{
				//数据归一化
				for (int i = 0; i < inNum; i++)
				{
					x[i] = p[isamp, i] / in_rate;
				}
				for (int i = 0; i < outNum; i++)
				{
					yd[i] = t[isamp, i] / in_rate;
				}

				//计算隐层的输入和输出

				for (int j = 0; j < hideNum; j++)
				{
					o1[j] = 0.0;
					for (int i = 0; i < inNum; i++)
					{
						o1[j] += w[i, j] * x[i];
					}
					x1[j] = 1.0 / (1.0 + Math.Exp(-o1[j] - b1[j]));
				}

				//计算输出层的输入和输出
				for (int k = 0; k < outNum; k++)
				{
					o2[k] = 0.0;
					for (int j = 0; j < hideNum; j++)
					{
						o2[k] += v[j, k] * x1[j];
					}
					x2[k] = 1.0 / (1.0 + Math.Exp(-o2[k] - b2[k]));
				}

				//计算输出层误差和均方差

				for (int k = 0; k < outNum; k++)
				{
					qq[k] = (yd[k] - x2[k]) * x2[k] * (1.0 - x2[k]);
					e += (yd[k] - x2[k]) * (yd[k] - x2[k]);
					//更新V
					for (int j = 0; j < hideNum; j++)
					{
						v[j, k] += rate * qq[k] * x1[j];
					}
				}

				//计算隐层误差

				for (int j = 0; j < hideNum; j++)
				{
					pp[j] = 0.0;
					for (int k = 0; k < outNum; k++)
					{
						pp[j] += qq[k] * v[j, k];
					}
					pp[j] = pp[j] * x1[j] * (1 - x1[j]);

					//更新W

					for (int i = 0; i < inNum; i++)
					{
						w[i, j] += rate * pp[j] * x[i];
					}
				}

				//更新b2
				for (int k = 0; k < outNum; k++)
				{
					b2[k] += rate * qq[k];
				}

				//更新b1
				for (int j = 0; j < hideNum; j++)
				{
					b1[j] += rate * pp[j];
				}

			}//end isamp
			e = Math.Sqrt(e);
			//      adjustWV(w,dw);
			//      adjustWV(v,dv);


		}//end train

		public void adjustWV(double[,] w, double[,] dw)
		{
			for (int i = 0; i < w.GetLength(0); i++)
			{
				for (int j = 0; j < w.GetLength(1); j++)
				{
					w[i, j] += dw[i, j];
				}
			}

		}

		public void adjustWV(double[] w, double[] dw)
		{
			for (int i = 0; i < w.Length; i++)
			{

				w[i] += dw[i];

			}
		}

		//数据仿真函数
		public double[] sim(double[] psim) //in_rate inNum HideNum outNum 
		{
			for (int i = 0; i < inNum; i++)
				x[i] = psim[i] / in_rate;

			for (int j = 0; j < hideNum; j++)
			{
				o1[j] = 0.0;
				for (int i = 0; i < inNum; i++)
					o1[j] = o1[j] + w[i, j] * x[i];
				x1[j] = 1.0 / (1.0 + Math.Exp(-o1[j] - b1[j]));
			}
			for (int k = 0; k < outNum; k++)
			{
				o2[k] = 0.0;
				for (int j = 0; j < hideNum; j++)
					o2[k] = o2[k] + v[j, k] * x1[j];
				x2[k] = 1.0 / (1.0 + Math.Exp(-o2[k] - b2[k]));

				x2[k] = in_rate * x2[k];

			}

			return x2;
		} //end sim

		public void initial() // 建立一些中间数组 tjt 预测新数据
		{
			x = new double[inNum];
			x1 = new double[hideNum];
			x2 = new double[outNum];

			o1 = new double[hideNum];
			o2 = new double[outNum];

			w = new double[inNum, hideNum];
			v = new double[hideNum, outNum];
			dw = new double[inNum, hideNum];
			dv = new double[hideNum, outNum];

			b1 = new double[hideNum];
			b2 = new double[outNum];
			db1 = new double[hideNum];
			db2 = new double[outNum];

			pp = new double[hideNum];
			qq = new double[outNum];
			yd = new double[outNum];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var stringpack = new StringPack();
			//return base.ToString();
			stringpack.W = new double[w.GetLength(0)][];
			for (int i = 0; i < stringpack.W.Length; i++)
			{
				stringpack.W[i] = new double[w.GetLength(1)];
				for (int j = 0; j < w.GetLength(1); j++)
				{
					stringpack.W[i][j] = w[i, j];
				}
			}

			stringpack.V = new double[v.GetLength(0)][];
			for (int i = 0; i < stringpack.V.Length; i++)
			{
				stringpack.V[i] = new double[v.GetLength(1)];
				for (int j = 0; j < v.GetLength(1); j++)
				{
					stringpack.V[i][j] = v[i, j];
				}
			}

			stringpack.B1 = b1;
			stringpack.B2 = b2;
			stringpack.iN = inNum;
			stringpack.hN = hideNum;
			stringpack.oN = outNum;
			stringpack.iR = in_rate;
			return JSON.Serialize(stringpack);
		}

	}//end bpnet
}
