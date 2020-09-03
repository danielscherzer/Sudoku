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
					for (int x = b0 * 3; x < 3 + b0 * 3; ++x)
					{
						for (int y = b1 * 3; y < 3 + b1 * 3; ++y)
						{
							var value = board[x, y];
							if (0 == value) continue;
							if (0 == used[value - 1])
							{
								firstUse[value - 1] = (x, y);
								++used[value - 1];
							}
							else
							{
								++used[value - 1];
								yield return (x, y);
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
			for (int x = 0; x < 9; ++x)
			{
				var used = new int[9];
				var firstUse = new int[9];
				for (int y = 0; y < 9; ++y)
				{
					var value = board[x, y];
					if (0 == value) continue;
					if (0 == used[value - 1])
					{
						firstUse[value - 1] = y;
						++used[value - 1];
					}
					else
					{
						++used[value - 1];
						yield return (x, y);
					}
				}
				// also yield return first cell of multiple occurrence
				for (int i = 0; i < 9; ++i)
				{
					if (1 < used[i])
					{
						yield return (x, firstUse[i]);
					}
				}
			}
		}

		private static IEnumerable<(int, int)> EnumerateAllInvalidCellsRows(int[,] board)
		{
			for (int y = 0; y < 9; ++y)
			{
				var used = new int[9];
				var firstUse = new int[9];
				for (int x = 0; x < 9; ++x)
				{
					var value = board[x, y];
					if (0 == value) continue;
					if (0 == used[value - 1])
					{
						firstUse[value - 1] = x;
						++used[value - 1];
					}
					else
					{
						++used[value - 1];
						yield return (x, y);
					}
				}
				// also yield return first cell of multiple occurrence
				for (int i = 0; i < 9; ++i)
				{
					if (1 < used[i])
					{
						yield return (firstUse[i], y);
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
