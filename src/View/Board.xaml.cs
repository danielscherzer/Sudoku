using System.Windows;
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

		public int ActiveValue
		{
			get { return (int)GetValue(ActiveValueProperty); }
			set { SetValue(ActiveValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ActiveValue.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ActiveValueProperty = DependencyProperty.Register("ActiveValue", typeof(int), typeof(Board), new PropertyMetadata(0));


		private void Cell_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var element = sender as Border;
			var cell = element?.DataContext as CellViewModel;
			if (DataContext is BoardViewModel board)
			{
				board.CurrentCell = cell;
				if (MouseButton.Right == e.ChangedButton && cell != null)
				{
					cell.Value = 0;
				}
			}
		}
	}
}
