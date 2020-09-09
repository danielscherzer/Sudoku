using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WpfSudoku.ViewModel
{
	public class CellViewModel : ViewModel<CellViewModel>
	{
		public CellViewModel(int column, int row)
		{
			Column = column;
			Row = row;
		}

		public bool this[uint index]
		{
			get
			{
				return Contains(index);
			}
			set
			{
				var bit = toBit[index];
				if (value)
				{
					_values |= bit;
				}
				else
				{
					_values &= ~bit;
				}
				InvokePropertyChanged();
				InvokePropertyChanged(nameof(PossibleValues));
			}
		}

		private bool _isActive = false;
		public bool IsActive
		{
			get => _isActive;
			set => Set(ref _isActive, value);
		}

		private bool _isInfluenced = false;
		public bool IsInfluenced
		{
			get => _isInfluenced;
			set => Set(ref _isInfluenced, value);
		}

		private bool _isReadOnly = false;
		public bool IsReadOnly
		{
			get => _isReadOnly;
			set => Set(ref _isReadOnly, value);
		}

		private bool _isValid = true;
		public bool IsValid
		{
			get => _isValid;
			set => Set(ref _isValid, value);
		}

		private uint _value = 0;
		public uint Value
		{
			get => _value;
			set
			{
				if (IsReadOnly) return;
				Set(ref _value, value);
			}
		}

		internal void Reset()
		{
			IsActive = false;
			IsInfluenced = false;
			IsReadOnly = false;
			IsValid = true;
			Value = 0;
			_values = toBit[10];
		}

		public IEnumerable<PossibleValue> PossibleValues
		{
			get
			{
				for (uint i = 1; i < 10; ++i)
				{
					yield return new PossibleValue(i, Contains(i));
				}
			}
		}

		public int Column { get; }
		public int Row { get; }

		public override string ToString() => $"{Value}:{(IsReadOnly ? "R" : "")}{(IsValid ? "" : "I")}";

		private static readonly uint[] toBit = { 0b0, 0b1, 0b10, 0b100, 0b1_000, 0b10_000, 0b100_000, 0b1_000_000, 0b10_000_000, 0b100_000_000, 0b111_111_111 };
		private uint _values = toBit[10];

		private bool Contains(uint value) => 0 != (toBit[value] & _values);
	}
}
