using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zenseless.Patterns;
using Zenseless.Spatial;

namespace WpfSudoku.Model
{
	public static class Sudoku
	{
		public static Grid<int> Create(int seed = 0)
		{
			if (-1 == seed) seed = (int)(DateTime.Now.Ticks % int.MaxValue);
			var rnd = new Random(seed);
			const int size = 9;
			const int ss = size * size;
			var field = new Grid<int>(size, size);
			var tries = new int[ss]; // try count for each cell
			var allChoicesRnd = new int[ss][]; //each cell gets a randomized order of trying the numbers 1-9
			for (int i = 0; i < ss; ++i)
			{
				var valueList = Enumerable.Range(1, 9).ToArray(); // create each new so allChoices has distinct arrays
				rnd.Shuffle(valueList);
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
						field.Cells[i] = 0;
						i -= 2;
						break;
					}
					field.Cells[i] = allChoicesRnd[i][tries[i] - 1];

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

		public static void RemoveSome(Grid<int> field, double propability, int seed = 0)
		{
			if (-1 == seed) seed = (int)(DateTime.Now.Ticks % int.MaxValue);
			var rnd = new Random(seed);
			for (int i = 0; i < field.Cells.Length; ++i)
			{
				if (rnd.NextDouble() < propability)
				{
					field.Cells[i] = 0;
				}
			}
		}

		public static bool SolveBacktrack(Grid<int> grid)
		{
			Debug.Assert(9 == grid.Columns);
			Debug.Assert(9 == grid.Rows);
			void PlaceNumber(int pos)
			{
				// check if we are beyond the last position in the grid
				if (pos == 81)
				{
					throw new ApplicationException("Finished!");
				}
				if (grid.Cells[pos] > 0)
				{
					//cell already filled -> go to next position in grid
					PlaceNumber(pos + 1);
				}
				else
				{
					var (column, row) = grid.GetColRow(pos);
					//cell not filled -> try all numbers
					for (int n = 1; n <= 9; ++n)
					{
						if (ValidityChecks.CheckCell(grid, column, row, n))
						{
							grid.Cells[pos] = n; // place a number and go to next cell
							PlaceNumber(pos + 1);
							grid.Cells[pos] = 0; // back track
						}
					}
				}
			}
			try
			{
				PlaceNumber(0);
				return false;
			}
			catch (ApplicationException)
			{
				return true;
			}
		}

		public static bool SolveBestFirst(Grid<int> grid)
		{
			Debug.Assert(9 == grid.Columns);
			Debug.Assert(9 == grid.Rows);
			(int, HashSet<int>) FindEmptyWithMinPossibleValues()
			{
				int min = int.MaxValue;
				int minId = -1;
				HashSet<int> result = new();
				for (int cell = 0; cell < grid.Cells.Length; ++cell)
				{
					if (0 == grid.Cells[cell])
					{
						var (column, row) = grid.GetColRow(cell);
						var validValues = ValidityChecks.ValidValues(grid, column, row);
						int count = validValues.Count;
						if (count < min)
						{
							min = count;
							minId = cell;
							result = validValues;
							if (1 == min) return (minId, result); // only one choice -> better is not possible
						}
					}
				}
				return (minId, result);
			}

			void PlaceNumber()
			{
				(int cell, var validValues) = FindEmptyWithMinPossibleValues();
				// check if no empty cells
				if (-1 == cell)
				{
					throw new ApplicationException("Finished!");
				}
				var (column, row) = grid.GetColRow(cell);
				//cell not filled -> try all valid values
				foreach (var n in validValues)
				{
					grid.Cells[cell] = n; // fill this cell and go to next free cell
					PlaceNumber();
					grid.Cells[cell] = 0;
				}
			}

			try
			{
				PlaceNumber();
				return false;
			}
			catch (ApplicationException)
			{
				return true;
			}
		}

		public static bool SolveExactCover(Grid<int> grid)
		{
			Debug.Assert(9 == grid.Columns);
			Debug.Assert(9 == grid.Rows);
			const int numberInColInRow = 9 * 9 * 9;
			const int constrains = 9 * 9 * 4;
			DLX solver = new(constrains, numberInColInRow);
			for (int row = 0, cell = 0; row < 9; ++row)
			{
				for (int column = 0; column < 9; ++column)
				{
					int box = row / 3 * 3 + column / 3;
					for (int digit = 0; digit < 9; ++digit)
					{
						//a row is a choice of a certain number at a certain place in the grid
						//each row has 4x"1" entry for each of the 4 constrains this choice satisfies:
						//columns 0ff: 81 cell constrains
						//columns 81ff: 81 row constrains
						//columns 2*81ff: 81 column constrains
						//columns 3*81ff: 81 box constrains
						solver.AddRow(cell, 81 + row * 9 + digit, 2 * 81 + column * 9 + digit, 3 * 81 + box * 9 + digit);
					}
					cell++;
				}
			}
			foreach(var row in solver.Rows())
			{
				//Debug.WriteLine(string.Join(',', row));
				//Debug.WriteLine(row[1] % 9 + 1);
				//Debug.WriteLine(row[2] % 9 + 1);
				//Debug.WriteLine(row[3] % 9 + 1);
			}
			for (int i = 0; i < 81; ++i)
			{
				var value = grid.Cells[i];
				if (0 != value) solver.Give(i * 9 + grid.Cells[i] - 1);
			}
			var solution = solver.Solutions().First();
			foreach (int r in solution)
			{
				grid.Cells[r / 81 * 9 + r / 9 % 9] = r % 9 + 1;
			}
			return true;
		}
	}
}
