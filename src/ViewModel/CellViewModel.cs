namespace WpfSudoku.ViewModel
{
	public class CellViewModel : ViewModel
	{
		private bool _isReadOnly = false;
		public bool IsReadOnly
		{
			get => _isReadOnly;
			set => Set(ref _isReadOnly, value);
		}

		private int _value = 0;
		public int Value
		{
			get => _value;
			set
			{
				if (IsReadOnly) return;
				Set(ref _value, value);
			}
		}

		private bool _isValid = true;
		public bool IsValid
		{
			get => _isValid;
			set => Set(ref _isValid, value);
		}

		private bool _isActive = false;
		public bool IsActive
		{
			get => _isActive;
			set => Set(ref _isActive, value);
		}

		public override string ToString() => $"{Value}:{(IsReadOnly ? "R" : "")}{(IsValid ? "" : "I")}";
	}
}
