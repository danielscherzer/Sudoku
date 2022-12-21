using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zenseless.Spatial;

namespace WpfSudoku.Model
{
	public static class ValidityChecks
	{
		public static bool CheckCell(IReadOnlyGrid<int> grid, int column, int row, int val)
		{
			Debug.Assert(9 == grid.Columns);
			Debug.Assert(9 == grid.Rows);
			// column/row check
			for (int i = 0; i < 9; ++i)
			{
				if (grid[i, row] == val || grid[column, i] == val)
					return false;
			}
			//box check
			int startX = (column / 3) * 3;
			int startY = (row / 3) * 3;
			for (int r = startY; r < startY + 3; ++r)
			{
				for (int c = startX; c < startX + 3; ++c)
				{
					if (grid[c, r] == val)
						return false;
				}
			}
			return true;
		}

		public static HashSet<int> ValidValues(IReadOnlyGrid<int> grid, int column, int row)
		{
			Debug.Assert(9 == grid.Columns);
			Debug.Assert(9 == grid.Rows);
			int[] fullSet = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var possibleChoices = new HashSet<int>(fullSet);
			if (grid[column, row] != 0)
			{
				// already value at cell -> nothing possible
				possibleChoices.Clear();
				return possibleChoices;
			}
			// column/row
			for (int i = 0; i < 9; ++i)
			{
				possibleChoices.Remove(grid[i, row]);
				possibleChoices.Remove(grid[column, i]);
			}
			//box check
			int startX = (column / 3) * 3;
			int startY = (row / 3) * 3;
			for (int r = startY; r < startY + 3; ++r)
			{
				for (int c = startX; c < startX + 3; ++c)
				{
					possibleChoices.Remove(grid[c, r]);
				}
			}
			return possibleChoices;
		}

		public static bool All(IReadOnlyGrid<int> board) => Boxes(board) && Columns(board) && Rows(board);

		public static bool Boxes(IReadOnlyGrid<int> board) => !EnumerateAllInvalidCellsBoxes(board).Any();

		public static bool Columns(IReadOnlyGrid<int> board) => !EnumerateAllInvalidCellsColumns(board).Any();

		public static bool Rows(IReadOnlyGrid<int> board) => !EnumerateAllInvalidCellsRows(board).Any();

		public static IEnumerable<(int, int)> EnumerateAllInvalidCells(IReadOnlyGrid<int> board)
		{
			foreach (var cell in EnumerateAllInvalidCellsBoxes(board)) yield return cell;
			foreach (var cell in EnumerateAllInvalidCellsColumns(board)) yield return cell;
			foreach (var cell in EnumerateAllInvalidCellsRows(board)) yield return cell;
		}

		private static IEnumerable<(int, int)> EnumerateAllInvalidCellsBoxes(IReadOnlyGrid<int> board)
		{
			for (int b0 = 0; b0 < 3; ++b0)
			{
				for (int b1 = 0; b1 < 3; ++b1)
				{
					var used = new int[9];
					var firstUse = new (int, int)[9];
					for (int column = b0 * 3; column < 3 + b0 * 3; ++column)
					{
						for (int row = b1 * 3; row < 3 + b1 * 3; ++row)
						{
							var value = board[column, row];
							if (0 == value) continue;
							if (0 == used[value - 1])
							{
								firstUse[value - 1] = (column, row);
								++used[value - 1];
							}
							else
							{
								++used[value - 1];
								yield return (column, row);
							}
						}
					}
					// also yield return first cell of multiple occurrence
					for (int i = 0; i < 9; ++i)
					{
						if (1 < used[i])
						{
							yield return firstUse[i];
						}
					}
				}
			}
		}

		private static IEnumerable<(int, int)> EnumerateAllInvalidCellsColumns(IReadOnlyGrid<int> board)
		{
			for (int column = 0; column < 9; ++column)
			{
				var used = new int[9];
				var firstUse = new int[9];
				for (int y = 0; y < 9; ++y)
				{
					var value = board[column, y];
					if (0 == value) continue;
					if (0 == used[value - 1])
					{
						firstUse[value - 1] = y;
						++used[value - 1];
					}
					else
					{
						++used[value - 1];
						yield return (column, y);
					}
				}
				// also yield return first cell of multiple occurrence
				for (int i = 0; i < 9; ++i)
				{
					if (1 < used[i])
					{
						yield return (column, firstUse[i]);
					}
				}
			}
		}

		private static IEnumerable<(int, int)> EnumerateAllInvalidCellsRows(IReadOnlyGrid<int> board)
		{
			for (int row = 0; row < 9; ++row)
			{
				var used = new int[9];
				var firstUse = new int[9];
				for (int column = 0; column < 9; ++column)
				{
					var value = board[column, row];
					if (0 == value) continue;
					if (0 == used[value - 1])
					{
						firstUse[value - 1] = column;
						++used[value - 1];
					}
					else
					{
						++used[value - 1];
						yield return (column, row);
					}
				}
				// also yield return first cell of multiple occurrence
				for (int i = 0; i < 9; ++i)
				{
					if (1 < used[i])
					{
						yield return (firstUse[i], row);
					}
				}
			}
		}
	}
}
