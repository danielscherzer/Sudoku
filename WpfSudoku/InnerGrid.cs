using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfSudoku
{
	public class InnerGrid : INotifyPropertyChanged
	{
		public InnerGrid(): this(3) { }

		public InnerGrid(int size)
		{
			for (int i = 0; i < size; i++)
			{
				ObservableCollection<Cell> Col = new ObservableCollection<Cell>();
				for (int j = 0; j < size; j++)
				{
					Cell c = new Cell();
					c.PropertyChanged += PropertyChangedHandler;
					Col.Add(c);
				}
				GridRows.Add(Col);
			}
		}

		public bool IsValid { get; private set; } = true;

		public Cell this[int row, int col]
		{
			get
			{
				if (row < 0 || row >= GridRows.Count) throw new ArgumentOutOfRangeException(nameof(row), row, "Invalid Row Index");
				if (col < 0 || col >= GridRows.Count) throw new ArgumentOutOfRangeException(nameof(col), col, "Invalid Column Index");
				return GridRows[row][col];
			}
		}

		public ObservableCollection<ObservableCollection<Cell>> GridRows { get; } = new ObservableCollection<ObservableCollection<Cell>>();

		public event PropertyChangedEventHandler PropertyChanged;

		void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Cell.Value))
			{
				bool valid = CheckIsValid();

				foreach (ObservableCollection<Cell> r in GridRows)
				{
					foreach (Cell c in r)
					{
						c.IsValid = valid;
					}
				}

				IsValid = valid;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
			}
		}

		private bool CheckIsValid()
		{
			bool[] used = new bool[GridRows.Count * GridRows.Count];
			foreach (ObservableCollection<Cell> r in GridRows)
			{
				foreach (Cell c in r)
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
			}
			return true;
		}
	}
}
