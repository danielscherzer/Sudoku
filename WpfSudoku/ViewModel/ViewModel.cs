using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WpfSudoku.ViewModel
{
	public class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		protected void Set<Value>([MaybeNull] ref Value backendStore, [MaybeNull] Value value, [CallerMemberName] string propertyName = "")
		{
			backendStore = value;
			InvokePropertyChanged(propertyName);
		}

		protected void InvokePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
