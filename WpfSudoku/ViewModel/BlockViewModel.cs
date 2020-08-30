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
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					CellViewModel cell = new CellViewModel
					{
						Value = i * size + j
					};
					if (0 != cell.Value) cell.IsReadOnly = true;
					cell.PropertyChanged += CellPropertyChanged;
					Items.Add(cell);
				}
			}
			Size = size;
		}

		public bool IsValid { get; private set; } = true;

		//public CellViewModel this[int row, int col]
		//{
		//	get
		//	{
		//		if (row < 0 || row >= Size) throw new ArgumentOutOfRangeException(nameof(row), row, "Invalid Row Index");
		//		if (col < 0 || col >= Size) throw new ArgumentOutOfRangeException(nameof(col), col, "Invalid Column Index");
		//		return Items[row * Size + col];
		//	}
		//}

		public ObservableCollection<CellViewModel> Items { get; } = new ObservableCollection<CellViewModel>();

		public int Size { get; }

		private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CellViewModel.Value))
			{
				var duplicate = FindDuplicate();

				foreach (CellViewModel cell in Items)
				{
					cell.IsValid = cell.Value != duplicate || 0 == cell.Value;
				}
				IsValid = 0 != duplicate;
				InvokePropertyChanged(nameof(IsValid));
			}
		}

		private int FindDuplicate()
		{
			bool[] used = new bool[Items.Count];
			foreach (CellViewModel c in Items)
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
