using Microsoft.UI.Xaml.Markup;

namespace WinUIApp.Markup
{
	internal sealed partial class EnumBinding : MarkupExtension
	{
		[AllowNull]
		public Type EnumType { get; set; }

		public int Skip { get; set; }

		protected override object ProvideValue()
		{
			IEnumerable<Enum> enums = Enum.GetValues(EnumType).OfType<Enum>();

			return Skip == 0 ? enums : (Skip > 0 ? enums.Skip(Skip) : enums.SkipLast(Math.Abs(Skip)));
		}
	}
}
