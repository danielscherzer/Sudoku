using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfSudoku.View
{
	class BooleanToFontWeightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var boolean = (bool)value;
			return boolean ? FontWeights.Bold : FontWeights.Normal;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
