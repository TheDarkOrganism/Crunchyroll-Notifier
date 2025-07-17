namespace WinUIApp.Models
{
	public sealed partial class DubTextInputModel : TextInputModelBase<DubTextInputModel>
	{
		private const string _displayName = "Dub";

		[Language(DisplayName = _displayName)]
		[NotEmpty(DisplayName = _displayName)]
		public override string Text { get => base.Text; set => base.Text = value; }
	}
}
