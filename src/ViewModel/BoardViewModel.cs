using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
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
					foreach (var cell in block.Cells) cell.PropertyChanged += CellPropertyChanged;
					Blocks.Add(block);
				}
			}
			Size = size;
			_ = FillAsync();
		}

		private CellViewModel? _currentCell;
		public CellViewModel? CurrentCell
		{
			get => _currentCell;
			set => Set(ref _currentCell, value);
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

		public async Task FillAsync()
		{
			var field = await SudokuCreator.FindAsync();
			SudokuCreator.RemoveSome(field, 0.3);
			ConvertField(field);
		}

		private void ConvertField(int[,] field)
		{
			var ss = Size * Size;
			for (int y = 0; y < ss; ++y)
			{
				for (int x = 0; x < ss; ++x)
				{
					var cell = this[y, x];
					cell.Reset();
					var value = field[x, y];
					if (0 != value)
					{
						cell.Value = (uint)value;
						cell.IsReadOnly = true;
					}
				}
			}
		}

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
				case nameof(CurrentCell): ActiveCellChanged(); break;
			}
		}

		private void ActiveCellChanged()
		{
			if (CurrentCell is null) return;

			if (CurrentCell.IsReadOnly)
			{
				if (0 == CurrentCell.Value) return;
				ActiveValue = CurrentCell.Value;
			}
			else
			{
				CurrentCell.Value = ActiveValue;
			}
		}

		private void ActiveValueChanged() => ForEachCell(cell => cell.IsActive = ActiveValue == cell.Value && cell.Value != 0);

		private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (nameof(CellViewModel.Value) == e.PropertyName)
			{
				ActiveValueChanged();
				RemoveFromPossibleValues();
				CheckValid();
			}
		}

		private void RemoveFromPossibleValues()
		{
			for (int y = 0; y < 9; ++y)
			{
				for (int x = 0; x < 9; ++x)
				{
					var cell = this[x, y];
					if (0 != cell.Value)
					{
						foreach((var u, var v) in SudokuCreator.InfluencedCoordinates(x, y))
						{
							this[u, v][cell.Value] = false;
						}
					}
				}
			}
		}

		private void CheckValid()
		{
			// convert
			var field = new int[9, 9];
			for (int y = 0; y < 9; ++y)
			{
				for (int x = 0; x < 9; ++x)
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
