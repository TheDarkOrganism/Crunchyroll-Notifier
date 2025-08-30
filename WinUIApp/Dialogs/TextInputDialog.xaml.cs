namespace WinUIApp.Dialogs
{
	internal sealed partial class TextInputDialog : ContentDialog
	{
		private readonly ITextInputModelBase _textInputModel;

		public TextInputDialog(UserActionType userAction, string name, int maxLength, ITextInputModelBase textInputModel)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
			ArgumentOutOfRangeException.ThrowIfLessThan(maxLength, 1, nameof(maxLength));

			_textInputModel = textInputModel;

			DataContext = _textInputModel;

			InitializeComponent();

			Title = $"{name} Prompt";

			string middle = userAction switch
			{
				UserActionType.add => "to",
				UserActionType.remove => "from",
				_ => throw new NotImplementedException()
			};

			MessageContent.Text = $"Enter a value to {userAction} {middle} {name}:";

			TextValueInput.MaxLength = maxLength;
		}

		private void TextValueInput_Loaded(object sender, RoutedEventArgs e)
		{
			TextValueInput.LoadValidation(_textInputModel);

			TextValueInput.Loaded -= TextValueInput_Loaded;
		}
	}
}
