namespace WinUIApp.Models
{
	public interface ITextInputModel : INotifyDataErrorInfo
	{
		string Text { get; set; }
	}
}
