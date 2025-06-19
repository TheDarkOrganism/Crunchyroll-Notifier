namespace WinUIApp.Models
{
	public sealed partial class TextInputModel : ValidationModelBase
	{
		private string _text = string.Empty;

		[NotEmpty]
		public string Text
		{
			get => _text;
			set
			{
				if (_text != value)
				{
					_text = value;

					OnPropertyChanged(value);
				}
			}
		}

		public TextInputModel()
		{
			ValidateProperty(Text);
		}
	}
}
