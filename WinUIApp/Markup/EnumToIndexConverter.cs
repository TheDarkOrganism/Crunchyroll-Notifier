using Microsoft.UI.Xaml.Data;

namespace WinUIApp.Markup
{
	internal sealed partial class EnumToIndexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return value is Enum result ? Enum.GetValues(result.GetType()).OfType<Enum>().ToList().IndexOf(result) : -1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
