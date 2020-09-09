using System.ComponentModel;

namespace WpfSudoku.ViewModel
{
	class PropertyChangedExtendedEventArgs<T>: PropertyChangedEventArgs
	{
		public T OldValue { get; }

		public PropertyChangedExtendedEventArgs(string propertyName, T oldValue) : base(propertyName) => OldValue = oldValue;
	}
}
