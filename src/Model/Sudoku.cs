using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WpfSudoku.Model
{
	public static class Sudoku
	{
		public static int[,] Find()
		{

			var timer = Stopwatch.StartNew();
			var field = CreateFullSearch();
			Helper.Log($"{timer.ElapsedMilliseconds}ms\n");
			return field;
		}

		public static async Task<int[,]> FindAsync()
		{
			return await Task.Run(() => Find());
		}

		public static void RemoveSome(int[,] field, double propability)
		{
			var rnd = new Random();
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

		private static int[,] CreateFullSearch()
		{
			var rnd = new Random();
			var field = new int[9, 9];
			var tries = new int[9 * 9];
			//each cell has its own order of trying the numbers 1-9
			var allChoicesRnd = new int[9 * 9][];
			for (int i = 0; i < 81; ++i)
			{
				var valueList = Enumerable.Range(1, 9).ToArray(); // performance side-note: this line is quicker inside of the for loop, then outside!?
				Shuffle(valueList, rnd);
				allChoicesRnd[i] = valueList;
			}
			static (int, int) Coord(int index) => (index % 9, index / 9);

			for(int i = 0; i < 81; ++i)
			{
				var (x, y) = Coord(i);
				do
				{
					tries[i]++;
					if (tries[i] > 9)
					{
						// tried all choices for this cell -> backtrack
						tries[i] = 0;
						field[x, y] = 0;
						i -= 2;
						break;
					}
					field[x, y] = allChoicesRnd[i][tries[i] - 1];

				} while (!ValidityChecks.All(field));
			}
			return field;
		}

		public static HashSet<int> SimplePossibleChoices(int[,] field, int x, int y, int blockSize)
		{
			int[] fullSet = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var possibleChoices = new HashSet<int>(fullSet);
			foreach ((var column, var row) in InterdependentFields(x, y, blockSize))
			{
				possibleChoices.Remove(field[column, row]);
			}
			return possibleChoices;
		}

		public static IEnumerable<(int, int)> InterdependentFields(int x, int y, int blockSize)
		{
			// row
			for (int column = 0; column < blockSize * blockSize; ++column)
			{
				if (x != column)
				{
					yield return (column, y);
				}
			}
			// column
			for (int row = 0; row < blockSize * blockSize; ++row)
			{
				if (y != row)
				{
					yield return (x, row);
				}
			}
			// box
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
