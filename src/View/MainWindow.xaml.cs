using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfSudoku.ViewModel;

namespace WpfSudoku.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			Board.PropertyChanged += Board_PropertyChanged;
		}

		public BoardViewModel Board { get; } = new BoardViewModel();

		public IEnumerable<string> Buttons => Enumerable.Range(1, 9).Select(i => i.ToString()).Prepend("Clear");

		private void Board_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(nameof(BoardViewModel.ActiveCell) == e.PropertyName)
			{
				if(Board.ActiveCell != null) Board.ActiveCell.Value = (uint)listBoxButtons.SelectedIndex;
			}
		}

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (System.Windows.Input.Key.Escape == e.Key) Close();
		}
	}
}
