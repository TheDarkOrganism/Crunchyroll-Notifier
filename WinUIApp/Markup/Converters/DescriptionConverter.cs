namespace WinUIApp.Markup.Converters
{
	internal sealed partial class DescriptionConverter : AttributeConverterBase<DescriptionAttribute>
	{
		protected override string? Convert(DescriptionAttribute? attribute, string propertyName)
		{
			return attribute?.Description;
		}
	}
}
