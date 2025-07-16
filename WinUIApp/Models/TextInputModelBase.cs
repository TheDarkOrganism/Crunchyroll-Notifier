namespace WinUIApp.Models
{
	public abstract class TextInputModelBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TModel> : ValidationModelBase<TModel>, ITextInputModel
	{
		private string _text = string.Empty;

		[NotEmpty]
		public virtual string Text
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

		protected TextInputModelBase()
		{
			ValidateProperty(Text);
		}
	}
}
