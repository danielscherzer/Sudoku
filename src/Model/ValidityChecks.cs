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
					var used = new bool[9];
					for (int x = b0 * 3; x < 3 + b0 * 3; ++x)
					{
						for (int y = b1 * 3; y < 3 + b1 * 3; ++y)
						{
							var value = board[x, y];
							if (0 == value) continue;
							if (used[value - 1])
							{
								yield return (x, y);
							}
							used[value - 1] = true;
						}
					}
				}
			}
		}

		private static IEnumerable<(int, int)> EnumerateAllInvalidCellsColumns(int[,] board)
		{
			for (int x = 0; x < 9; ++x)
			{
				var used = new bool[9];
				for (int y = 0; y < 9; ++y)
				{
					var value = board[x, y];
					if (0 == value) continue;
					if (used[value - 1])
					{
						yield return (x, y);
					}
					used[value - 1] = true;
				}
			}
		}

		private static IEnumerable<(int, int)> EnumerateAllInvalidCellsRows(int[,] board)
		{
			for (int y = 0; y < 9; ++y)
			{
				var used = new bool[9];
				for (int x = 0; x < 9; ++x)
				{
					var value = board[x, y];
					if (0 == value) continue;
					if (used[value - 1])
					{
						yield return (x, y);
					}
					used[value - 1] = true;
				}
			}
		}

		internal static bool All(int[,] board) => Boxes(board) && Columns(board) && Rows(board);

		internal static bool Boxes(int[,] board) => !EnumerateAllInvalidCellsBoxes(board).Any();

		internal static bool Columns(int[,] board) => !EnumerateAllInvalidCellsColumns(board).Any();

		internal static bool Rows(int[,] board) => !EnumerateAllInvalidCellsRows(board).Any();
	}
}
