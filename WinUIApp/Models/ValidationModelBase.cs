using System.Runtime.CompilerServices;

namespace WinUIApp.Models
{
	internal abstract class ValidationModelBase<TModel> : ModelBase, IValidationModelBase
		where TModel : notnull, ValidationModelBase<TModel>, new()
	{
		private static readonly Dictionary<string, ValidationAttribute[]> _validationAttributes = typeof(TModel).GetValidationAttributes();

		private static readonly Dictionary<string, string> _friendlyNames = _validationAttributes.Keys.ToDictionary(static key => key, static key => key.AddSpaces());

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

		protected bool IsValid<TValue>(TValue value, string propertyName)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

			return !_validationAttributes.TryGetValue(propertyName, out ValidationAttribute[]? validationAttributes) || validationAttributes.All(attribute => attribute.IsValid(value));
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

		protected void OnPropertyChanged<TValue>(TValue value, bool reloadConfiguration = false, [CallerMemberName, NotNull] string? propertyName = null)
		{
			ValidateProperty(value, propertyName);

			OnPropertyChanged(propertyName);

			OnModified(reloadConfiguration);
		}

		public IEnumerable GetErrors(string? propertyName)
		{
			return propertyName is null
			? _errors.SelectMany(static pair => pair.Value).ToArray().AsReadOnly()
			: _errors.TryGetValue(propertyName, out IReadOnlyCollection<string>? value) ? value : [];
		}
	}
}
