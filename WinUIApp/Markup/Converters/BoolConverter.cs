namespace WinUIApp.Markup.Converters
{
	internal sealed partial class BoolConverter : IValueConverter
	{
		private static readonly CultureInfo _culture = CultureInfo.InvariantCulture;

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			switch (value)
			{
				case bool boolValue:
					return boolValue;
				case IConvertible convertible:
					try
					{
						return convertible.ToBoolean(_culture);
					}
					catch
					{
						return false;
					}
				default:
					return false;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (value is bool boolValue)
			{
				try
				{
					return System.Convert.ChangeType(boolValue, targetType, _culture);
				}
				catch
				{
					return value;
				}
			}
			else
			{
				return value;
			}
		}
	}
}
