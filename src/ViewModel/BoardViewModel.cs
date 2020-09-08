using System.Collections.Generic;
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
			// create cells
			Size = size;
			var ss = Size * Size;
			for (int row = 0; row < ss; ++row)
			{
				for (int column = 0; column < ss; ++column)
				{
					var cell = new CellViewModel(column, row);
					cell.PropertyChanged += CellPropertyChanged;
					_cells.Add(cell);
				}
			}
			//create blocks
			var blocks = new List<BlockViewModel>();
			for (int b1 = 0; b1 < Size; ++b1)
			{
				for (int b0 = 0; b0 < Size; ++b0)
				{
					var blockCells = new List<CellViewModel>(9);
					for (int row = b1 * Size; row < Size + b1 * Size; ++row)
					{
						for (int column = b0 * Size; column < Size + b0 * Size; ++column)
						{
							blockCells.Add(GetCell(column, row));
						}
					}
					var block = new BlockViewModel(blockCells, Size);
					foreach (var cell in block.Cells) cell.PropertyChanged += CellPropertyChanged;
					blocks.Add(block);
				}
			}
			Blocks = blocks;
			PropertyChanged += BoardViewModel_PropertyChanged;
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

		private readonly List<CellViewModel> _cells = new List<CellViewModel>();
		private CellViewModel GetCell(int col, int row) => _cells[col + Size * Size * row];

		public IEnumerable<BlockViewModel> Blocks { get; }

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
			for (int column = 0; column < ss; ++column)
			{
				for (int row = 0; row < ss; ++row)
				{
					var cell = GetCell(column, row);
					cell.Reset();
					var value = field[column, row];
					if (0 != value)
					{
						cell.Value = (uint)value;
						cell.IsReadOnly = true;
					}
				}
			}
		}

		//private BlockViewModel GetBlock(int row, int col)
		//{
		//	if (row < 0 || row >= Size) throw new ArgumentOutOfRangeException(nameof(row), row, "Invalid Row Index");
		//	if (col < 0 || col >= Size) throw new ArgumentOutOfRangeException(nameof(col), col, "Invalid Column Index");
		//	return Blocks[row * Size + col];
		//}

		private void BoardViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(ActiveValue): ActiveValueChanged(); break;
				case nameof(CurrentCell): CurrentCellChanged(); break;
			}
		}

		private void CurrentCellChanged()
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

		private void ActiveValueChanged()
		{
			foreach (var cell in _cells)
			{
				cell.IsActive = ActiveValue == cell.Value && cell.Value != 0;
			}
		}

		private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (nameof(CellViewModel.Value) == e.PropertyName)
			{
				ActiveValueChanged();
				//RemoveFromPossibleValues();
				CheckValid();
			}
		}

		private void RemoveFromPossibleValues()
		{
			var ss = Size * Size;
			for (int column = 0; column < ss; ++column)
			{
				for (int row = 0; row < ss; ++row)
				{
					var cell = GetCell(column, row);
					if (0 != cell.Value)
					{
						foreach((var u, var v) in SudokuCreator.InfluencedCoordinates(column, row))
						{
							GetCell(u, v)[cell.Value] = false;
						}
					}
				}
			}
		}

		private void CheckValid()
		{
			// convert
			var field = new int[9, 9];
			for (int column = 0; column < 9; ++column)
			{
				for (int row = 0; row < 9; ++row)
				{
					var cell = GetCell(column, row);
					field[column, row] = (int)cell.Value;
					cell.IsValid = true;
				}
			}
			foreach((var column, var row) in ValidityChecks.EnumerateAllInvalidCells(field))
			{
				GetCell(column, row).IsValid = false;
			}
			// is won?
			var won = true;
			foreach (var cell in _cells)
			{
				won &= cell.IsValid && 0 != cell.Value;
			}
			IsWon = won;
		}
	}
}
