using System;
using System.Collections.ObjectModel;

namespace WpfSudoku.ViewModel
{
	public class BlockViewModel : ViewModel
	{
		public BlockViewModel() : this(9) { }

		public BlockViewModel(int size)
		{
			for (int i = 0; i < size * size; i++)
			{
				CellViewModel cell = new CellViewModel();
				Cells.Add(cell);
			}
			Size = size;
		}

		public CellViewModel this[int row, int col]
		{
			get
			{
				if (row < 0 || row >= Size) throw new ArgumentOutOfRangeException(nameof(row), row, "Invalid Row Index");
				if (col < 0 || col >= Size) throw new ArgumentOutOfRangeException(nameof(col), col, "Invalid Column Index");
				return Cells[row * Size + col];
			}
		}

		public ObservableCollection<CellViewModel> Cells { get; } = new ObservableCollection<CellViewModel>();

		public int Size { get; }
	}
}
