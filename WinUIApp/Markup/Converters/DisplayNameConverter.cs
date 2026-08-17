namespace WinUIApp.Markup.Converters
{
	internal sealed partial class DisplayNameConverter : AttributeConverterBase<DisplayNameAttribute>
	{
		protected override string? Convert(DisplayNameAttribute? attribute, string propertyName)
		{
			return attribute?.DisplayName ?? propertyName;
		}
	}
}
