using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WpfSudoku.Model
{
	public class SudokuCreator
	{
		public static int[,] Find()
		{

			var timer = Stopwatch.StartNew();
			var field = CreateIncrementPattern();
			Helper.Log($"{timer.ElapsedMilliseconds}ms\n");
			return field;
		}

		public static async Task<int[,]> FindAsync()
		{
			return await Task.Run(() => Find());
		}

		public static void RemoveSome(int[,] field, double propability)
		{
			for (int x = 0; x < field.GetLength(0); ++x)
			{
				for (int y = 0; y < field.GetLength(1); ++y)
				{
					if (rnd.NextDouble() < propability)
					{
						field[x, y] = 0;
					}
				}
			}
		}

		static readonly Random rnd = new Random();

		private static int[,] CreateIncrementPattern()
		{
			var field = new int[9, 9];
			int value = 0;
			void process(ref int b)
			{
				value %= 9;
				value++;
				b = value;
			}
			bool valid = true;
			int counter = 0;
			do
			{
				field.ForEach(process, () => value += 3 + rnd.Next(10));
				valid = ValidityChecks.All(field);
				counter++;
			}
			while (!valid);
			Helper.Log($"Try= {counter}; ");
			return field;
		}

		private static int[,] CreateShuffledLines()
		{
			var field = new int[9, 9];
			int[] shuffledLine = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			int counter = 0;
			const int maxCounter = 1_000_000;
			for (int y = 0; y < 8;)
			{
				Shuffle(shuffledLine);
				++counter;
				if (counter > maxCounter) break;
				//copy line
				for (int x = 0; x < 9; ++x) field[x, y] = shuffledLine[x];
				if (ValidityChecks.All(field)) y++;
			}
			// last line
			for (int x = 0; x < 9; ++x)
			{
				var unused = new HashSet<int>(shuffledLine);
				for (int y = 0; y < 8; ++y)
				{
					unused.Remove(field[x, y]);
				}
				field[x, 8] = unused.First();
			}
			if (counter > maxCounter) Helper.Log("Overflow; ");
			else Helper.Log($"ShuffledLines={counter}; ");
			return field;
		}

		private static int[,] CreateBacktrack()
		{
			var possibleFields = new HashSet<int>[9, 9];
			int[] fullSet = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			possibleFields.ForEach((ref HashSet<int> e) => e = new HashSet<int>(fullSet));
			for (int y = 0; y < 9; ++y)
			{
				Shuffle(fullSet);
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

		public static IEnumerable<(int, int)> InfluencedCoordinates(int x, int y, int blockSize)
		{
			// update row
			for (int column = 0; column < blockSize * blockSize; ++column)
			{
				if (x != column)
				{
					yield return (column, y);
				}
			}
			// update column
			for (int row = 0; row < blockSize * blockSize; ++row)
			{
				if (y != row)
				{
					yield return (x, row);
				}
			}
			// update box
			var u0 = (x / blockSize) * blockSize;
			var v0 = (y / blockSize) * blockSize;
			for (int u = u0; u < blockSize + u0; ++u)
			{
				for (int v = v0; v < blockSize + v0; ++v)
				{
					if (u == x && v == y) continue;
					yield return (u, v);
				}
			}
		}
		private static void Shuffle(int[] numbers)
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
