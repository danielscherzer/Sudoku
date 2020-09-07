namespace WpfSudoku.ViewModel
{
	public struct PossibleValue
	{
		public PossibleValue(uint value, bool isValid)
		{
			IsValid = isValid;
			Value = value;
		}

		public bool IsValid { get; }
		public uint Value { get; }
	}
}