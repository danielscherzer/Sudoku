using System.Windows.Controls;
using System.Windows.Input;
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

		private void Cell_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var element = sender as Border;
			var cell = element?.DataContext as CellViewModel;
			if (DataContext is BoardViewModel board)
			{
				board.ActiveCell = cell;
				if (MouseButton.Right == e.ChangedButton && cell != null)
				{
					cell.Value = 0;
				}
			}
		}
	}
}
