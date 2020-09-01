using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Sudoku
{
	internal static class Helper
	{
		internal delegate void Process<Type>(ref Type value);

		internal static void ForEach<Type>(this Type[,] field, Process<Type> process, Action? newRow = null)
		{
			for (byte y = 0; y < 9; ++y)
			{
				for (byte x = 0; x < 9; ++x)
				{
					process(ref field[x, y]);
				}
				newRow?.Invoke();
			}
		}

		internal static void Print(this byte[,] field)
		{
			var sb = new StringBuilder();
			var separatorLine = new string('-', 9 + 3);
			sb.AppendLine(separatorLine);
			byte column = 0;
			void process(ref byte b)
			{
				sb.Append(b);
				column++;
				if (0 == column % 3)
				{
					sb.Append('|');
				}
			}

			byte row = 0;
			void newRow()
			{
				sb.AppendLine();
				row++;
				if (0 == row % 3)
				{
					sb.AppendLine(separatorLine);
				}
			}

			ForEach(field, process, newRow);
			Console.Write(sb);
		}
	}
}
