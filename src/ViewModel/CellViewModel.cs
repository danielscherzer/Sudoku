using System;
using System.Diagnostics.CodeAnalysis;

namespace WpfSudoku.ViewModel
{
	public class CellViewModel : ViewModel, IEquatable<CellViewModel>
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

		public bool Equals([AllowNull] CellViewModel other) => other?.Value == Value;

		public override string ToString() => $"{Value}:{(IsReadOnly ? "R" : "")}{(IsValid ? "" : "I")}";
	}
}
