using Sudoku;
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
			PropertyChanged += BoardViewModel_PropertyChanged;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					var block = new BlockViewModel(size);
					//block.PropertyChanged += BlockPropertyChanged;
					foreach (var cell in block.Cells) cell.PropertyChanged += CellPropertyChanged;
					Blocks.Add(block);
				}
			}
			//cells = new CellViewModel[size, size];

			Size = size;
			FillBoard();
		}

		private CellViewModel? _activeCell;
		public CellViewModel? ActiveCell
		{
			get => _activeCell;
			set => Set(ref _activeCell, value);
		}

		private int _activeValue;
		public int ActiveValue
		{
			get => _activeValue;
			set => Set(ref _activeValue, value);
		}

		public CellViewModel this[int row, int col]
		{
			get
			{
				if (row < 0 || row >= Size * Size) throw new ArgumentOutOfRangeException(nameof(row), row, "Invalid Row Index");
				if (col < 0 || col >= Size * Size) throw new ArgumentOutOfRangeException(nameof(col), col, "Invalid Column Index");
				var block = GetBlock(row / Size, col / Size);
				return block[row % 3, col % 3];
			}
		}

		public ObservableCollection<BlockViewModel> Blocks { get; } = new ObservableCollection<BlockViewModel>();
		public int Size { get; }

		//private CellViewModel[,] cells;

		private BlockViewModel GetBlock(int row, int col)
		{
			if (row < 0 || row >= Size) throw new ArgumentOutOfRangeException(nameof(row), row, "Invalid Row Index");
			if (col < 0 || col >= Size) throw new ArgumentOutOfRangeException(nameof(col), col, "Invalid Column Index");
			return Blocks[row * Size + col];
		}

		private void BoardViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (nameof(ActiveValue) == e.PropertyName) UpdateCellActive();
		}

		private void UpdateCellActive() => ForEachCell(cell => cell.IsActive = ActiveValue == cell.Value && cell.Value != 0);

		private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (nameof(CellViewModel.Value) == e.PropertyName) UpdateCellActive();
		}

		private void FillBoard()
		{
			var rnd = new Random();
			var field = SudokuCreator.Find();
			var ss = Size * Size;
			for (byte y = 0; y < ss; ++y)
			{
				for (byte x = 0; x < ss; ++x)
				{
					if (rnd.NextDouble() > 0.2)
					{
						var cell = this[x, y];
						cell.Value = field[x, y];
						cell.IsReadOnly = true;
					}
				}
			}
		}

		private void ForEachCell(Action<CellViewModel> action)
		{
			foreach (var block in Blocks)
			{
				foreach (var cell in block.Cells)
				{
					action(cell);
				}
			}
		}
	}
}
