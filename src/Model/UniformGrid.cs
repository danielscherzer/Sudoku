using System;
using System.Diagnostics.CodeAnalysis;

namespace WpfSudoku.Model
{
	public class UniformGrid<T>
	{
		public UniformGrid(int size)
		{
			Array = new T[size * size];
			Size = size;
		}

		public UniformGrid([NotNull] T[] array, int size, bool copy)
		{
			Array = array ?? throw new ArgumentNullException(nameof(array));
			if (copy)
			{
				Array = new T[size * size];
				array.CopyTo(array, 0);
			}
			if (array.Length != size * size) throw new ArgumentException($"{nameof(array)} length={array.Length} does not match size={size} squared.");
			Size = size;
		}

		public UniformGrid([NotNull] T[,] array)
		{
			if (array.GetLength(0) != array.GetLength(1)) throw new ArgumentException($"{nameof(array)} is not square.");
			Size = array.GetLength(0);
			Array = new T[Size * Size];
			int i = 0;
			for (int row = 0; row < Size; ++row)
			{
				for (int column = 0; column < Size; ++column)
				{
					Array[i] = array[column, row];
					++i;
				}
			}
		}
		public T this[int column, int row]
		{
			get => Array[Index(column, row)];
			set => Array[Index(column, row)] = value;
		}

		public T[] Array;

		public int Size { get; }

		public (int, int) Coord(int index) => (index % Size, index / Size);

		public int Index(int column, int row) => column + Size * row;

		public T[,] To2dArray()
		{
			var array = new T[Size, Size];
			int i = 0;
			for (int row = 0; row < Size; ++row)
			{
				for (int column = 0; column < Size; ++column)
				{
					array[column, row] = Array[i];
					++i;
				}
			}
			return array;
		}
	}
}
