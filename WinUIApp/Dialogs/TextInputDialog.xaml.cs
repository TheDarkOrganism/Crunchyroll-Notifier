namespace WinUIApp.Dialogs
{
	internal sealed partial class TextInputDialog : ContentDialog
	{
		private readonly ITextInputModel _textInputModel;

		public bool IsValid => !_textInputModel.HasErrors;

		public string Text => _textInputModel.Text;

		public TextInputDialog(UserActionType userAction, string name, int maxLength, ITextInputModel textInputModel)
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
