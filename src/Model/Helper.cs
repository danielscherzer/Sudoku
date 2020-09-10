using System;
using System.IO;

namespace WpfSudoku.Model
{
	internal static class Helper
	{
		internal static void Log(string msg)
		{
#if DEBUG
			Console.Write(msg);
			//File.AppendAllText(@"d:\bench.log", msg);
#endif
		}
	}
}
