namespace WpfSudoku
{
	public class CellViewModel : ViewModel
	{

		bool _readOnly = false;
		public bool ReadOnly
		{
			get => _readOnly;
			set => Set(ref _readOnly, value);
		}

		int _value = 0;
		public int Value
		{
			get => _value;
			set => Set(ref _value, value);
		}

		bool _isValid = true;
		public bool IsValid
		{
			get => _isValid;
			set => Set(ref _isValid, value);
		}
	}
}
