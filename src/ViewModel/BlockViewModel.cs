using System;
using System.Collections.Generic;

namespace WpfSudoku.ViewModel
{
	public class BlockViewModel : ViewModel
	{
		public BlockViewModel(IEnumerable<CellViewModel> cells, int size)
		{
			int count = 0;
			foreach(var cell in cells)
			{
				_cells.Add(cell);
				++count;
			}
			Size = size;
			if (Size * Size != count) throw new ArgumentOutOfRangeException(nameof(cells), count, $"Invalid number of cells given for size={size}");
		}

		//public CellViewModel this[int col, int row]
		//{
		//	get
		//	{
		//		if (row < 0 || row >= Size) throw new ArgumentOutOfRangeException(nameof(row), row, "Invalid Row Index");
		//		if (col < 0 || col >= Size) throw new ArgumentOutOfRangeException(nameof(col), col, "Invalid Column Index");
		//		return _cells[row * Size + col];
		//	}
		//}

		public IEnumerable<CellViewModel> Cells => _cells;

		public int Size { get; }

		private readonly List<CellViewModel> _cells = new List<CellViewModel>();
	}
}
