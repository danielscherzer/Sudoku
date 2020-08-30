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
		}

		public ObservableCollection<BlockViewModel> Blocks { get; } = new ObservableCollection<BlockViewModel>();

		private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}
	}
}
