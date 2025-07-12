using System.Runtime.CompilerServices;

namespace WinUIApp.Models
{
	public abstract class ValidationModelBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TModel> : ModelBase, INotifyDataErrorInfo, INotifyPropertyChanged
	{
		private static readonly Dictionary<string, ValidationAttribute[]> _validationAttributes = typeof(TModel).GetValidationAttributes();

		private static readonly Dictionary<string, string> _friendlyNames = _validationAttributes.Keys.ToDictionary(static key => key, static key =>
		{
			int length = key.Length;

			int newLength = length + key.Skip(1).Count(char.IsUpper);

			if (length == newLength)
			{
				return key;
			}

			return string.Create(newLength, key, static (span, state) =>
			{
				ReadOnlySpan<char> stateSpan = state;

				int offset = 0;

				for (int i = 0; i < stateSpan.Length; i++)
				{
					char c = stateSpan[i];

					if (i > 0 && char.IsUpper(c))
					{
						span[i + offset] = ' ';

						offset++;
					}

					span[i + offset] = c;
				}
			});
		});

		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
		public event PropertyChangedEventHandler? PropertyChanged;

		private readonly Dictionary<string, IReadOnlyCollection<string>> _errors = [];

		[JsonIgnore]
		public bool HasErrors => _errors.Count > 0;

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new(propertyName));
		}

		private void OnErrorsChanged(string propertyName)
		{
			ErrorsChanged?.Invoke(this, new(propertyName));
			OnPropertyChanged(nameof(HasErrors));
		}

		protected bool ContainsErrors([CallerMemberName, NotNull] string? propertyName = null)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

			return _errors.ContainsKey(propertyName);
		}

		protected void ValidateProperty<TValue>(TValue value, [CallerArgumentExpression(nameof(value)), NotNull] string? propertyName = null)
		{
			if (ContainsErrors(propertyName) && _errors.Remove(propertyName))
			{
				OnErrorsChanged(propertyName);
			}

			if (_validationAttributes.TryGetValue(propertyName, out ValidationAttribute[]? validationAttributes))
			{
				List<string> errors = [];

				foreach (ValidationAttribute validationAttribute in validationAttributes)
				{
					if (!validationAttribute.IsValid(value))
					{
						errors.Add(validationAttribute.FormatErrorMessage(_friendlyNames.TryGetValue(propertyName, out string? friendlyName) ? friendlyName : propertyName));
					}
				}

				if (errors.Count > 0)
				{
					_errors.Add(propertyName, errors.AsReadOnly());

					OnErrorsChanged(propertyName);
				}
			}
		}

		protected void OnPropertyChanged<TValue>(TValue value, [CallerMemberName] string? propertyName = null)
		{
			ValidateProperty(value, propertyName);

			OnPropertyChanged(propertyName);

			OnModified();
		}

		public IEnumerable GetErrors(string? propertyName)
		{
			return propertyName is null
			? _errors.SelectMany(static pair => pair.Value).ToArray().AsReadOnly()
			: _errors.TryGetValue(propertyName, out IReadOnlyCollection<string>? value) ? value : [];
		}
	}
}
