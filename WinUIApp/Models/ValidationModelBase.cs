namespace WinUIApp.Models
{
	internal abstract class ValidationModelBase<TModel> : ModelBase, IValidationModelBase
		where TModel : notnull, ValidationModelBase<TModel>
	{
		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
		public event PropertyChangedEventHandler? PropertyChanged;

		private readonly Dictionary<string, IReadOnlyCollection<string>> _errors = [];

		[JsonIgnore]
		public bool HasErrors => _errors.Count > 0;

		protected bool ContainsErrors([CallerMemberName, NotNull] string? propertyName = null)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

			return _errors.ContainsKey(propertyName);
		}

		private void OnPropertyChanged(string propertyName, bool modified)
		{
			PropertyChanged?.Invoke(this, new(propertyName));

			if (modified)
			{
				Validate();

				OnModified();
			}
		}

		private void OnErrorsChanged(string propertyName)
		{
			ErrorsChanged?.Invoke(this, new(propertyName));
			OnPropertyChanged(nameof(HasErrors), false);
		}

		protected abstract ValidateOptionsResult ValidateOptions();

		private void Validate()
		{
			foreach (string key in _errors.Keys)
			{
				if (_errors.Remove(key))
				{
					OnErrorsChanged(key);
				}
			}

			ValidateOptionsResult result = ValidateOptions();

			if (result.Failed && result.Failures is IEnumerable<string> failures)
			{
				foreach (string failure in failures)
				{
					string[] parts = failure.Split(':', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

					if (parts.Length != 2)
					{
						continue;
					}

					string key = parts[0];

					List<string> errors = _errors.TryGetValue(key, out IReadOnlyCollection<string>? value) ? new(value) : [];

					string message = parts[1];

					int index = message.IndexOf(key, StringComparison.InvariantCulture);

					if (index > -1)
					{
						message = $"{message[..index]}{key.AddSpaces()}{message[(index + key.Length)..]}";
					}

					errors.Add(message);

					_errors[key] = errors.AsReadOnly();

					OnErrorsChanged(key);
				}
			}
		}

		protected void OnPropertyChanged([CallerMemberName, NotNull] string? propertyName = null)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

			OnPropertyChanged(propertyName, true);
		}

		public IEnumerable GetErrors(string? propertyName)
		{
			return propertyName is null
			? _errors.SelectMany(static pair => pair.Value).ToArray().AsReadOnly()
			: _errors.TryGetValue(propertyName, out IReadOnlyCollection<string>? value) ? value : [];
		}

		protected ValidationModelBase()
		{
			Validate();
		}
	}
}
