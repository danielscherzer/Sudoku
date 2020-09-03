using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WpfSudoku.Model;

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
			Size = size;
			FillBoard();
		}

		private CellViewModel? _activeCell;
		public CellViewModel? ActiveCell
		{
			get => _activeCell;
			set => Set(ref _activeCell, value);
		}

		private uint _activeValue;
		public uint ActiveValue
		{
			get => _activeValue;
			set => Set(ref _activeValue, value);
		}

		private CellViewModel this[int row, int col]
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

		private bool _isWon = false;
		public bool IsWon
		{
			get => _isWon;
			set => Set(ref _isWon, value);
		}

		public int Size { get; }

		private BlockViewModel GetBlock(int row, int col)
		{
			if (row < 0 || row >= Size) throw new ArgumentOutOfRangeException(nameof(row), row, "Invalid Row Index");
			if (col < 0 || col >= Size) throw new ArgumentOutOfRangeException(nameof(col), col, "Invalid Column Index");
			return Blocks[row * Size + col];
		}

		private void BoardViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(ActiveValue): ActiveValueChanged(); break;
				case nameof(ActiveCell): ActiveCellChanged(); break;
			}
		}

		private void ActiveCellChanged()
		{
			if (ActiveCell is null) return;

			if (!ActiveCell.IsReadOnly)
			{
				ActiveCell.Value = ActiveValue;
			}
			else
			{
				if (0 == ActiveCell.Value) return;
				ActiveValue = ActiveCell.Value;
			}
		}

		private void ActiveValueChanged() => ForEachCell(cell => cell.IsActive = ActiveValue == cell.Value && cell.Value != 0);

		private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (nameof(CellViewModel.Value) == e.PropertyName)
			{
				ActiveValueChanged();
				CheckValid();
			}
		}

		private void CheckValid()
		{
			// convert
			var field = new int[9, 9];
			for (int x = 0; x < 9; ++x)
			{
				for (int y = 0; y < 9; ++y)
				{
					field[y, x] = (int)this[x, y].Value;
					this[x, y].IsValid = true;
				}
			}
			foreach((var x, var y) in ValidityChecks.EnumerateAllInvalidCells(field))
			{
				this[y, x].IsValid = false;
			}
			// is won?
			var won = true;
			ForEachCell(cell => won &= cell.IsValid && 0 != cell.Value);
			IsWon = won;
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
					if (rnd.NextDouble() > 0.3)
					{
						var cell = this[x, y];
						cell.Value = (uint)field[x, y];
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
