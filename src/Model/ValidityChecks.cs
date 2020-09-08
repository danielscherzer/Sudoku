using System.Collections.Generic;
using System.Linq;

namespace WpfSudoku.Model
{
	internal class ValidityChecks
	{
		internal static IEnumerable<(int, int)> EnumerateAllInvalidCells(int[,] board)
		{
			foreach (var cell in EnumerateAllInvalidCellsBoxes(board)) yield return cell;
			foreach (var cell in EnumerateAllInvalidCellsColumns(board)) yield return cell;
			foreach (var cell in EnumerateAllInvalidCellsRows(board)) yield return cell;
		}

		private static IEnumerable<(int, int)> EnumerateAllInvalidCellsBoxes(int[,] board)
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

		private static IEnumerable<(int, int)> EnumerateAllInvalidCellsColumns(int[,] board)
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

		private static IEnumerable<(int, int)> EnumerateAllInvalidCellsRows(int[,] board)
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

		internal static bool All(int[,] board) => Boxes(board) && Columns(board) && Rows(board);

		internal static bool Boxes(int[,] board) => !EnumerateAllInvalidCellsBoxes(board).Any();

		internal static bool Columns(int[,] board) => !EnumerateAllInvalidCellsColumns(board).Any();

		internal static bool Rows(int[,] board) => !EnumerateAllInvalidCellsRows(board).Any();
	}
}
