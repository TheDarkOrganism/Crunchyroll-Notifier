namespace WinUIApp.Models
{
	public sealed partial class DubTextInputModel : TextInputModelBase<DubTextInputModel>
	{
		[Language]
		public override string Text { get => base.Text; set => base.Text = value; }
	}
}
