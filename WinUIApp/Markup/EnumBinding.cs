using Microsoft.UI.Xaml.Markup;

namespace WinUIApp.Markup
{
	internal sealed partial class EnumBinding : MarkupExtension
	{
		[AllowNull]
		public Type EnumType { get; set; }

		protected override object ProvideValue()
		{
			return Enum.GetValues(EnumType).OfType<Enum>();
		}
	}
}
