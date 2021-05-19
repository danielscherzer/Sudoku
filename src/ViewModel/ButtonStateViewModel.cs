using Zenseless.Patterns;

namespace WpfSudoku.ViewModel
{
	public class ButtonStateViewModel : PropertyBinding<ButtonStateViewModel>
	{
		public ButtonStateViewModel(string text)
		{
			Text = text;
		}

		public uint Count
		{
			get => _count;
			set
			{
				Set(ref _count, value);
				RaisePropertyChanged(nameof(IsFull));
			}
		}
		public bool IsFull => 9 == Count;
		public string Text { get; }

		public override string ToString() => $"{Text}:{Count}";

		private uint _count = 0;
	}
}