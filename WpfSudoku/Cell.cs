using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfSudoku
{
	public class Cell : INotifyPropertyChanged
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

		public event PropertyChangedEventHandler PropertyChanged;

		private void Set<Value>(ref Value backendStore, Value value, [CallerMemberName] string propertyName = "") where Value : IEquatable<Value>
		{
			if (!backendStore.Equals(value))
			{
				backendStore = value;
				InvokePropertyChanged(propertyName);
			}
		}

		private void InvokePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
