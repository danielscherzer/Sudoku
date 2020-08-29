using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfSudoku
{
	public class GridViewModel<Cell> : ViewModel where Cell : INotifyPropertyChanged, new()
	{
		public GridViewModel() : this(3) { }

		public GridViewModel(int size)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					var cell = new Cell();
					cell.PropertyChanged += CellPropertyChanged;
					Items.Add(cell);
				}
			}
		}

		public bool IsValid { get; private set; } = true;

		public ObservableCollection<Cell> Items { get; } = new ObservableCollection<Cell>();

		void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}
	}
}
