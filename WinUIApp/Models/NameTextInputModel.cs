namespace WinUIApp.Models
{
	public sealed partial class NameTextInputModel : TextInputModelBase<NameTextInputModel>, INameTextInputModel
	{
		[NotEmpty(DisplayName = "Name")]
		public override string Text { get => base.Text; set => base.Text = value; }
	}
}
