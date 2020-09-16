using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfSudoku.Model
{
	public static class Sudoku
	{
		public static UniformGrid<int> Create()
		{
			var rnd = new Random();
			const int size = 9;
			const int ss = size * size;
			var field = new UniformGrid<int>(size);
			var tries = new int[ss];
			//each cell has its own order of trying the numbers 1-9
			var allChoicesRnd = new int[ss][];
			for (int i = 0; i < ss; ++i)
			{
				var valueList = Enumerable.Range(1, 9).ToArray(); // performance side-note: this line is quicker inside of the for loop, then outside!?
				Shuffle(valueList, rnd);
				allChoicesRnd[i] = valueList;
			}

			for (int i = 0; i < ss; ++i)
			{
				do
				{
					tries[i]++;
					if (tries[i] > 9)
					{
						// tried all choices for this cell -> backtrack
						tries[i] = 0;
						field.Array[i] = 0;
						i -= 2;
						break;
					}
					field.Array[i] = allChoicesRnd[i][tries[i] - 1];

				} while (!ValidityChecks.All(field));
			}
			return field;
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

		public static void RemoveSome(UniformGrid<int> field, double propability)
		{
			var rnd = new Random();
			for (int i = 0; i < field.Size * field.Size; ++i)
			{
				if (rnd.NextDouble() < propability)
				{
					field.Array[i] = 0;
				}
			}
		}

		public static HashSet<int> SimplePossibleChoices(UniformGrid<int> field, int x, int y, int blockSize)
		{
			int[] fullSet = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var possibleChoices = new HashSet<int>(fullSet);
			foreach ((var column, var row) in InterdependentFields(x, y, blockSize))
			{
				possibleChoices.Remove(field[column, row]);
			}
			return possibleChoices;
		}

		public static void Solve(UniformGrid<int> field)
		{
			// only need to solve empty cells
			var emptyFieldIds = new List<int>();
			for (int i = 0; i < field.Size * field.Size; ++i)
			{
				if(0 == field.Array[i])
				{
					emptyFieldIds.Add(i);
				}
			}

			var choices = new int[emptyFieldIds.Count];
			for (int i = 0; i < emptyFieldIds.Count; ++i)
			{
				var currentId = emptyFieldIds[i];
				do
				{
					choices[i]++;
					if (choices[i] > 9)
					{
						// tried all choices for this cell -> backtrack
						choices[i] = 0;
						field.Array[currentId] = 0;
						i -= 2;
						break;
					}
					field.Array[currentId] = choices[i];

				} while (!ValidityChecks.All(field));
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
