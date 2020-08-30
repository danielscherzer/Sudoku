using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfSudoku.ViewModel
{
	public class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void Set<Value>(ref Value backendStore, Value value, [CallerMemberName] string propertyName = "") where Value : IEquatable<Value>
		{
			if (!backendStore.Equals(value))
			{
				backendStore = value;
				InvokePropertyChanged(propertyName);
			}
		}

		protected void InvokePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
