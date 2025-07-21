namespace WinUIApp.Models
{
	public abstract class TextInputModelBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TModel> : ValidationModelBase<TModel>, ITextInputModelBase
		where TModel : notnull, ValidationModelBase<TModel>, new()
	{
		private string _text = string.Empty;

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
