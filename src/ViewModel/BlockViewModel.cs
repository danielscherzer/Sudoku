using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
				cell.PropertyChanged += CellPropertyChanged;
				Cells.Add(cell);
			}
			Size = size;
		}

		public bool IsValid { get; private set; } = true;

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

		private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CellViewModel.Value))
			{
				var duplicate = FindDuplicate();

				foreach (CellViewModel cell in Cells)
				{
					cell.IsValid = cell.Value != duplicate || 0 == cell.Value;
				}
				IsValid = 0 != duplicate;
				InvokePropertyChanged(nameof(IsValid));
			}
		}

		private uint FindDuplicate()
		{
			bool[] used = new bool[Cells.Count];
			foreach (CellViewModel c in Cells)
			{
				if (0 != c.Value)
				{
					if (used[c.Value - 1])
					{
						return c.Value; //this is a duplicate
					}
					else
					{
						used[c.Value - 1] = true;
					}
				}
			}
			return 0;
		}
	}
}
