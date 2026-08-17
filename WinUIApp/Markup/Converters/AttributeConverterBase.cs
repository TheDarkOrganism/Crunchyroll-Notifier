namespace WinUIApp.Markup.Converters
{
	internal abstract class AttributeConverterBase<TAttribute> : IAttributeConverter
		where TAttribute : notnull, Attribute
	{
		public Type? DataContext { get; set; }

		protected abstract string? Convert(TAttribute? attribute, string propertyName);

		public string Convert(IEnumerable<Attribute> attributes, string propertyName)
		{
			return Convert(attributes.OfType<TAttribute>().FirstOrDefault(), propertyName) ?? string.Empty;
		}
	}
}
