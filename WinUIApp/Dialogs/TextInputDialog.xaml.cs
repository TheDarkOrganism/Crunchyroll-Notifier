using Microsoft.UI.Xaml.Controls;
using WinUIApp.Enums;

namespace WinUIApp.Dialogs
{
	internal sealed partial class TextInputDialog : ContentDialog
	{
		private readonly TextInputModel _textInputModel;

		public string Text => _textInputModel.Text;

		public TextInputDialog(UserActionType userAction, string text, int maxLength)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(text, nameof(text));
			ArgumentOutOfRangeException.ThrowIfLessThan(maxLength, 1, nameof(maxLength));

			_textInputModel = new();

			DataContext = _textInputModel;

			InitializeComponent();

			Title = $"{userAction} Prompt";

			MessageContent.Text = $"Enter a value to {text} {userAction}:";

			TextValueInput.MaxLength = maxLength;
		}
	}
}
