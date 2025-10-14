namespace WinUIApp.Markup.Converters
{
	internal sealed partial class EnumToIndexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is not Enum result)
			{
				return -1;
			}

			IEnumerable<Enum> enumValues = Enum.GetValues(result.GetType()).OfType<Enum>();

			if (int.TryParse(parameter as string, out int skip))
			{
				enumValues = enumValues.DynamicSkip(skip);
			}

			return enumValues.ToList().IndexOf(result);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
