using System.Windows.Controls;
using WpfSudoku.ViewModel;

namespace WpfSudoku.View
{
	/// <summary>
	/// Interaction logic for SudokuBoard.xaml
	/// </summary>
	public partial class Board : UserControl
	{
		public Board()
		{
			InitializeComponent();
		}

		private void Cell_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var element = sender as Border;
			var cell = element?.DataContext as CellViewModel;
			if (DataContext is BoardViewModel board) board.ActiveCell = cell;
		}
	}
}
