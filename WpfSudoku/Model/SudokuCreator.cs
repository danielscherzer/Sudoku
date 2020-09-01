using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Sudoku
{
	class SudokuCreator
	{
		static readonly ThreadLocal<Random> rnd = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

		public static void Try() => ThreadPool.QueueUserWorkItem(_ => Find());

		public static void TryMany()
		{
			for(int i = 0; i < Environment.ProcessorCount; ++i)
			{
				ThreadPool.QueueUserWorkItem(ThreadProcMany);
			}
		}

		private static void ThreadProcMany(object? _)
		{
			while (true)
			{
				Find();
			}
		}

		public static byte[,] Find()
		{
			var timer = Stopwatch.StartNew();
			var random = rnd.Value;
			if (random is null)
			{
				return new byte[9, 9];
			}
			byte[,] field;
			bool valid;
			int counter = 0;
			do
			{
				field = Create(random);
				valid = ValidityChecks.All(field);
				counter++;
			} while (!valid);
			field.Print();
			Console.WriteLine($"Try= {counter} in {timer.ElapsedMilliseconds}ms; {counter / timer.ElapsedMilliseconds}try/msec");
			//Console.ReadKey();
			return field;
		}

		private static byte[,] Create(Random rnd)
		{
			var field = new byte[9, 9];
			byte value = 0;
			void process(ref byte b)
			{
				value %= 9;
				value++;
				b = value;
			}
			field.ForEach(process, () => value += (byte)(3 + rnd.Next()));
			return field;
		}

		private static byte[,] Create2(Random rnd)
		{
			var field = new byte[9, 9];
			byte[] shuffledLine = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			for (byte y = 0; y < 9; ++y)
			{
				Shuffle(shuffledLine, rnd);
				for (byte x = 0; x < 9; ++x)
				{
					field[x, y] = shuffledLine[x];
				}
			}
			return field;
		}

		private static byte[,] Create3(Random rnd)
		{
			var possibleFields = new HashSet<byte>[9, 9];
			byte[] fullSet = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			possibleFields.ForEach((ref HashSet<byte> e) => e = new HashSet<byte>(fullSet));
			for (byte y = 0; y < 9; ++y)
			{
				Shuffle(fullSet, rnd);
				for (byte x = 0; x < 9; ++x)
				{
					Update(possibleFields, x, y, fullSet[x]);
				}
			}
			return new byte[9, 9];
		}

		private static void Update(HashSet<byte>[,] possibleFields, byte x, byte y, byte value)
		{
			possibleFields[x, y] = new HashSet<byte>() { value };
			// update row
			for (byte column = 0; column < 9; ++column)
			{
				if (x != column)
				{
					possibleFields[column, y].Remove(value);
				}
			}
			// update column
			for (byte row = 0; row < 9; ++row)
			{
				if (y != row)
				{
					possibleFields[x, row].Remove(value);
				}
			}
			// update box
			var u0 = (x % 3) * 3;
			var v0 = (y % 3) * 3;
			for (int u = u0; u < 3 + u0; ++u)
			{
				for (int v = v0; v < 3 + v0; ++v)
				{
					if (u != x || v != y)
					{
						possibleFields[u, v].Remove(value);
					}
				}
			}
		}

		private static void Shuffle(byte[] numbers, Random rnd)
		{
			for (int i = numbers.Length - 1; i > 0; i--)
			{
				int rndId = rnd.Next(i);
				var temp = numbers[i];
				numbers[i] = numbers[rndId];
				numbers[rndId] = temp;
			}
		}
	}
}
