using Microsoft.UI.Xaml.Controls;

namespace WinUIApp.Dialogs
{
	internal sealed partial class TextInputDialog : ContentDialog
	{
		private readonly TextInputModel _textInputModel;

		public string Text => _textInputModel.Text;

		public TextInputDialog(string name, string text, int maxLength)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
			ArgumentException.ThrowIfNullOrWhiteSpace(text, nameof(text));
			ArgumentOutOfRangeException.ThrowIfLessThan(maxLength, 1, nameof(maxLength));

			_textInputModel = new();

			DataContext = _textInputModel;

			InitializeComponent();

			Title = $"{name} Prompt";

			MessageContent.Text = $"Enter a value to {text} {name}:";

			TextValueInput.MaxLength = maxLength;
		}
	}
}
