using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfSudoku
{
	public class BlockViewModel : ViewModel
	{
		public BlockViewModel(): this(9) { }

		public BlockViewModel(int size)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					CellViewModel cell = new CellViewModel
					{
						Value = i * size + j + 1
					};
					cell.PropertyChanged += CellPropertyChanged;
					Items.Add(cell);
				}
			}
		}

		public bool IsValid { get; private set; } = true;

		//public Cell this[int row, int col]
		//{
		//	get
		//	{
		//		if (row < 0 || row >= Items.Count) throw new ArgumentOutOfRangeException(nameof(row), row, "Invalid Row Index");
		//		if (col < 0 || col >= Items.Count) throw new ArgumentOutOfRangeException(nameof(col), col, "Invalid Column Index");
		//		return Items[row][col];
		//	}
		//}

		public ObservableCollection<CellViewModel> Items { get; } = new ObservableCollection<CellViewModel>();

		void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CellViewModel.Value))
			{
				bool valid = CheckIsValid();

				foreach (CellViewModel cell in Items)
				{
					cell.IsValid = valid;
				}

				IsValid = valid;
				InvokePropertyChanged(nameof(IsValid));
			}
		}

		private bool CheckIsValid()
		{
			bool[] used = new bool[Items.Count * Items.Count];
			foreach (CellViewModel c in Items)
			{
				if (0 != c.Value)
				{
					if (used[c.Value - 1])
					{
						return false; //this is a duplicate
					}
					else
					{
						used[c.Value - 1] = true;
					}
				}
			}
			return true;
		}
	}
}
