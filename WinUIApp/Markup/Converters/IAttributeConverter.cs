namespace WinUIApp.Markup.Converters
{
	internal interface IAttributeConverter
	{
		Type? DataContext { get; }

		string Convert(IEnumerable<Attribute> attributes, string propertyName);
	}
}
