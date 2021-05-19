using System.Collections.Generic;
using System.Linq;
using Zenseless.Patterns;

namespace WpfSudoku.ViewModel
{
	public class MainViewModel : PropertyBinding<MainViewModel>
	{
		public MainViewModel()
		{
			_activeNumbers = Enumerable.Range(1, 9).Select(i => new ButtonStateViewModel(i.ToString())).Prepend(new ButtonStateViewModel("Clear")).ToArray();
			foreach (var block in Board.Blocks)
			{
				foreach (var cell in block.Cells)
				{
					cell.AddPropertyChangedHandler(nameof(CellViewModel.Value), CellValueChanged);
				}
			}
		}
		public IEnumerable<ButtonStateViewModel> ActiveNumbers => _activeNumbers;

		public BoardViewModel Board { get; } = new BoardViewModel();

		private readonly ButtonStateViewModel[] _activeNumbers;

		private void CellValueChanged()
		{
			for (int i = 0; i < _activeNumbers.Length; ++i)
			{
				_activeNumbers[i].Count = 0;
			}
			foreach (var block in Board.Blocks)
			{
				foreach (var cell in block.Cells)
				{
					if (0 != cell.Value && cell.IsValid) _activeNumbers[cell.Value].Count++;
				}
			}
			RaisePropertyChanged(nameof(ActiveNumbers));
		}
	}
}
