using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfSudoku.ViewModel
{
	public class BoardViewModel : ViewModel
	{
		public BoardViewModel() : this(3) { }

		public BoardViewModel(int size)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					var cell = new BlockViewModel(size);
					cell.PropertyChanged += CellPropertyChanged;
					Blocks.Add(cell);
				}
			}
			Size = size;
		}

		private CellViewModel? _activeCell;
		public CellViewModel? ActiveCell
		{
			get => _activeCell;
			set => Set(ref _activeCell, value);
		}

		public BlockViewModel this[int row, int col]
		{
			get
			{
				if (row < 0 || row >= Size) throw new ArgumentOutOfRangeException(nameof(row), row, "Invalid Row Index");
				if (col < 0 || col >= Size) throw new ArgumentOutOfRangeException(nameof(col), col, "Invalid Column Index");
				return Blocks[row * Size + col];
			}
		}

		public ObservableCollection<BlockViewModel> Blocks { get; } = new ObservableCollection<BlockViewModel>();
		public int Size { get; }

		private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}
	}
}
