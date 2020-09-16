using System;
using System.Diagnostics;
using System.IO;

namespace WpfSudoku.Model
{
	internal static class Helper
	{
		internal static void Benchmark(Action action)
		{
			var timer = Stopwatch.StartNew();
			action();
			Log($"{timer.ElapsedMilliseconds}ms\n");
		}

		internal static void Log(string msg)
		{
#if DEBUG
			Console.Write(msg);
			//File.AppendAllText(@"d:\bench.log", msg);
#endif
		}
	}
}
