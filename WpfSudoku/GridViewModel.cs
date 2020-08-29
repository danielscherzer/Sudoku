using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfSudoku
{
	public class GridViewModel : ViewModel
	{
		public GridViewModel() : this(3) { }

		public GridViewModel(int size)
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
		}

		public bool IsValid { get; private set; } = true;

		public ObservableCollection<BlockViewModel> Blocks { get; } = new ObservableCollection<BlockViewModel>();

		void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}
	}
}
