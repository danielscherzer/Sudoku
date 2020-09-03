using System;
using System.Collections.Generic;
using System.Threading;

namespace WpfSudoku.Model
{
	class SudokuCreator
	{
		static readonly ThreadLocal<Random> rnd = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

		public static void Try() => ThreadPool.QueueUserWorkItem(_ => Find());

		public static void TryMany()
		{
			for (int i = 0; i < Environment.ProcessorCount; ++i)
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

		public static int[,] Find()
		{
			var random = rnd.Value;
			if (random is null)
			{
				return new int[9, 9];
			}
			int[,] field;
			bool valid;
			int counter = 0;
			do
			{
				field = Create(random);
				valid = ValidityChecks.All(field);
				counter++;
			} while (!valid);
			return field;
		}

		private static int[,] Create(Random rnd)
		{
			var field = new int[9, 9];
			int value = 0;
			void process(ref int b)
			{
				value %= 9;
				value++;
				b = value;
			}
			field.ForEach(process, () => value += 3 + rnd.Next());
			return field;
		}

		private static int[,] Create2(Random rnd)
		{
			var field = new int[9, 9];
			int[] shuffledLine = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			for (int y = 0; y < 9; ++y)
			{
				Shuffle(shuffledLine, rnd);
				for (int x = 0; x < 9; ++x)
				{
					field[x, y] = shuffledLine[x];
				}
			}
			return field;
		}

		private static int[,] Create3(Random rnd)
		{
			var possibleFields = new HashSet<int>[9, 9];
			int[] fullSet = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			possibleFields.ForEach((ref HashSet<int> e) => e = new HashSet<int>(fullSet));
			for (int y = 0; y < 9; ++y)
			{
				Shuffle(fullSet, rnd);
				for (int x = 0; x < 9; ++x)
				{
					Update(possibleFields, x, y, fullSet[x]);
				}
			}
			return new int[9, 9];
		}

		private static void Update(HashSet<int>[,] possibleFields, int x, int y, int value)
		{
			possibleFields[x, y] = new HashSet<int>() { value };
			// update row
			for (int column = 0; column < 9; ++column)
			{
				if (x != column)
				{
					possibleFields[column, y].Remove(value);
				}
			}
			// update column
			for (int row = 0; row < 9; ++row)
			{
				if (y != row)
				{
					possibleFields[x, row].Remove(value);
				}
			}
			// update box
			var u0 = x % 3 * 3;
			var v0 = y % 3 * 3;
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

		private static void Shuffle(int[] numbers, Random rnd)
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
