namespace WinUIApp.Models
{
	internal interface ITextInputModelBase : IValidationModelBase
	{
		string Text { get; set; }
	}
}
