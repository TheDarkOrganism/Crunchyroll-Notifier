using Microsoft.UI.Xaml.Markup;

namespace WinUIApp.Markup.Binding
{
	internal sealed partial class EnumBinding : MarkupExtension
	{
		[AllowNull]
		public Type EnumType { get; set; }

		public int Skip { get; set; }

		protected override object ProvideValue()
		{
			return Enum.GetValues(EnumType).OfType<Enum>().DynamicSkip(Skip);
		}
	}
}
