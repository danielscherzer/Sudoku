using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using WpfSudoku.Model;

namespace WpfSudoku.ViewModel
{
	public class BoardViewModel : ViewModel<BoardViewModel>
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
					cell.AddPropertyChangedHandler(() => cell.Value, CellValueChanged);
					_cells.Add(cell); // order of adding is important first along column, than rows
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
							blockCells.Add(GetCell(column, row)); // order of adding is important first along column, than rows
						}
					}
					var block = new BlockViewModel(blockCells, Size);
					blocks.Add(block); // order of adding is important first along column, than rows
				}
			}
			Blocks = blocks;
			AddPropertyChangedHandler(nameof(ActiveValue), ActiveValueChanged);
			AddPropertyChangedHandler(nameof(EditCell), EditCellChanged);
			_ = FillAsync();
		}

		private CellViewModel? _editCell;
		public CellViewModel? EditCell
		{
			get => _editCell;
			set => Set(ref _editCell, value);
		}

		private uint _activeValue;
		public uint ActiveValue
		{
			get => _activeValue;
			set => Set(ref _activeValue, value);
		}

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
			var field = await Task.Run(() =>
			{
				int[,] field = new int[9, 9];
				Helper.Benchmark(() => field = Sudoku.Find());
				return field;
			});
			Sudoku.RemoveSome(field, 0.5);
			ConvertField(field);
		}

		public async Task SolveAsync()
		{
			var field = ToField();
			await Task.Run(() => Sudoku.Solve(field));
			ConvertField(field);
		}

		private readonly List<CellViewModel> _cells = new List<CellViewModel>();
		private CellViewModel GetCell(int col, int row) => _cells[col + Size * Size * row];

		private void ConvertField(int[,] field)
		{
			var ss = Size * Size;
			for (int row = 0; row < ss; ++row)
			{
				for (int column = 0; column < ss; ++column)
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

		private void EditCellChanged()
		{
			if (EditCell is null) return;

			if (EditCell.IsReadOnly)
			{
				ActiveValue = EditCell.Value;
			}
			else
			{
				EditCell.Value = ActiveValue;
			}
		}

		private void ActiveValueChanged()
		{
			foreach (var cell in _cells)
			{
				UpdateCellActive(cell);
			}
		}

		private void CellValueChanged(CellViewModel cell, uint oldValue)
		{
			UpdateCellActive(cell);
			if (0 != cell.Value)
			{
				foreach ((var u, var v) in Sudoku.InterdependentFields(cell.Column, cell.Row, Size))
				{
					GetCell(u, v)[cell.Value] = false;
					//Helper.Log($"{u},{v}\n");
				}
			}
			CheckValid();
			CheckWon();
		}

		private void CheckWon()
		{
			var won = true;
			foreach (var cell in _cells)
			{
				won &= cell.IsValid && 0 != cell.Value;
			}
			IsWon = won;
		}

		private void CheckValid()
		{
			foreach (var cell in _cells)
			{
				cell.IsValid = true;
			}
			var field = ToField();
			foreach ((var column, var row) in ValidityChecks.EnumerateAllInvalidCells(field))
			{
				GetCell(column, row).IsValid = false;
			}
		}

		private int[,] ToField()
		{
			var field = new int[9, 9];
			foreach (var cell in _cells)
			{
				field[cell.Column, cell.Row] = (int)cell.Value;
			}
			return field;
		}

		private void UpdateCellActive(CellViewModel cell) => cell.IsActive = ActiveValue == cell.Value && cell.Value != 0;
	}
}
