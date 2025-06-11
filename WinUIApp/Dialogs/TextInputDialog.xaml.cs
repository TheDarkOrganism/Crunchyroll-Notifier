namespace WinUIApp.Dialogs
{
	internal sealed partial class TextInputDialog : ContentDialog
	{
		private readonly TextInputModel _textInputModel;

		public string Text => _textInputModel.Text;

		public TextInputDialog(UserActionType userAction, string name, int maxLength)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
			ArgumentOutOfRangeException.ThrowIfLessThan(maxLength, 1, nameof(maxLength));

			_textInputModel = new();

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
	}
}
