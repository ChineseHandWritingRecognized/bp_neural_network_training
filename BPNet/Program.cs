using BPNet.Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPNet
{
	class Program
	{
		static void Main(string[] args)
		{
			//总训练次数
			const int alltimes = 10000;
			Console.Title = "";
			//单独读取笔画数据
			var traindata = new TrainData[]{
				new TrainData("Res/Strokes/S00.json",0),
				new TrainData("Res/Strokes/S01.json",1),
				new TrainData("Res/Strokes/S02.json",2),
				new TrainData("Res/Strokes/S03.json",3),
				new TrainData("Res/Strokes/S04.json",4),
				new TrainData("Res/Strokes/S05.json",5),
				new TrainData("Res/Strokes/S06.json",6)
			};

			#region 合并数据到一起
			var data_num = 0;
			foreach (var item in traindata)
			{
				data_num += item.MatrixData.GetLength(0);
			}

			var sumdata = new TrainData();
			sumdata.MatrixData = new double[data_num, traindata[0].MatrixData.GetLength(1)];
			sumdata.Anwser = new double[data_num, traindata[0].Anwser.GetLength(1)];

			int startpoint = 0;
			for (int i = 0; i < traindata.Length; i++)
			{
				var item = traindata[i];
				for (int k = 0; k < item.MatrixData.GetLength(0); k++)
				{
					for (int j = 0; j < item.MatrixData.GetLength(1); j++)
					{
						sumdata.MatrixData[startpoint, j] = item.MatrixData[k, j];
					}
					for (int j = 0; j < item.Anwser.GetLength(1); j++)
					{
						sumdata.Anwser[startpoint, j] = item.Anwser[k, j];
					}
					startpoint++;
				}
			}
			#endregion

			Console.WriteLine("加载数据完成,开始训练...");

			//把"问题"和"答案"交给神经网络,让神经网络自己研究怎么把"问题"计算成"答案"
			var bp = new BpNet(sumdata.MatrixData, sumdata.Anwser);

			//循环进行多次训练
			for (int i = 1; i < alltimes; i++)
			{
				bp.train(sumdata.MatrixData, sumdata.Anwser);
				if (i % (alltimes / 10) == 0) Console.WriteLine($"总训练次数:{alltimes} 当前训练次数:{i}");
			}

			Console.WriteLine("训练完成,开始预测..");

			//加载测试数据集
			var test = Extends.GetDim1Matrix("Res/test.json");

			//循环把测试数据集放进去识别:
			for (int i = 0; i < test.GetLength(0); i++)
			{
				var sim0 = bp.sim(test.GetColumn(i));
				int like = 0;
				var distant = double.MaxValue;
				//排序.把最接近1的排到上面
				for (int k = 0; k < sim0.Length; k++)
				{
					if (Math.Abs(sim0[k] - 1) > distant) continue;
					distant = Math.Abs(sim0[k] - 1);
					like = k;
				}
				Console.WriteLine($"测试数据{i}识别结果:{like}");
			}
			var result = bp.ToString();
			//把训练出来的权重数据保存到JSON
			File.WriteAllText("GData.json", result);

			Console.ReadKey(true);
		}

	}
}
