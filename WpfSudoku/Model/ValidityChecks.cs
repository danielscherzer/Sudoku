namespace Sudoku
{
	internal class ValidityChecks
	{
		internal static bool All(byte[,] field)
		{
			return Boxes(field) && Columns(field) && Rows(field);
		}

		internal static bool Boxes(byte[,] field)
		{
			var used = new bool[9];
			for (int b0 = 0; b0 < 3; ++b0)
			{
				for (int b1 = 0; b1 < 3; ++b1)
				{
					for (int x = b0 * 3; x < 3 + b0 * 3; ++x)
					{
						for (int y = b1 * 3; y < 3 + b1 * 3; ++y)
						{
							var value = field[x, y];
							if (0 == value) continue;
							if (used[value - 1])
							{
								//Debug.WriteLine($"box {b0}x{b1} multiple element {value}");
								return false;
							}
							used[value - 1] = true;
						}
					}
					used = new bool[9];
				}
			}
			return true;
		}

		internal static bool Columns(byte[,] field)
		{
			var used = new bool[9];
			for (byte x = 0; x < 9; ++x)
			{
				for (byte y = 0; y < 9; ++y)
				{
					var value = field[x, y];
					if (0 == value) continue;
					if (used[value - 1])
					{
						//Debug.WriteLine($"column {x + 1} multiple element {value}");
						return false;
					}
					used[value - 1] = true;
				}
				used = new bool[9];
			}
			return true;
		}

		internal static bool Rows(byte[,] field)
		{
			var used = new bool[9];
			for (byte y = 0; y < 9; ++y)
			{
				for (byte x = 0; x < 9; ++x)
				{
					var value = field[x, y];
					if (0 == value) continue;
					if(used[value - 1])
					{
						//Debug.WriteLine($"row {y + 1} multiple element {value}");
						return false;
					}
					used[value - 1] = true;
				}
				used = new bool[9];
			}
			return true;
		}
	}
}
