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
			viewModel = new MainViewModel();
			DataContext = viewModel;
			InitializeComponent();
			//CompositionTarget.Rendering += async (s, a) => await viewModel.Board.FillAsync();
		}

		private readonly MainViewModel viewModel;

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (System.Windows.Input.Key.Escape == e.Key) Close();
		}

		private async void ButtonNew_Click(object sender, RoutedEventArgs e) => await viewModel.Board.FillAsync();

		private async void ButtonSolve_Click(object sender, RoutedEventArgs e) => await viewModel.Board.SolveAsync();
	}
}
