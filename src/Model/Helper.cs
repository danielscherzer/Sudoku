﻿using System;
using System.IO;

namespace WpfSudoku.Model
{
	internal static class Helper
	{
		internal delegate void Process<Type>(ref Type value);

		internal static void ForEach<Type>(this Type[,] field, Process<Type> process, Action? newRow = null)
		{
			for (byte y = 0; y < 9; ++y)
			{
				for (byte x = 0; x < 9; ++x)
				{
					process(ref field[x, y]);
				}
				newRow?.Invoke();
			}
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
