using System;
using System.Diagnostics.CodeAnalysis;

namespace WpfSudoku.ViewModel
{
	public class CellViewModel : ViewModel, IEquatable<CellViewModel>
	{
		private bool _readOnly = false;
		public bool ReadOnly
		{
			get => _readOnly;
			set => Set(ref _readOnly, value);
		}

		private int _value = 0;
		public int Value
		{
			get => _value;
			set => Set(ref _value, value);
		}

		private bool _isValid = true;
		public bool IsValid
		{
			get => _isValid;
			set => Set(ref _isValid, value);
		}

		public bool Equals([AllowNull] CellViewModel other) => other?.Value == Value;
	}
}
