using System.Collections;
using System.Runtime.CompilerServices;

namespace WinUIApp.Models
{
	public abstract class ValidationModelBase : ModelBase, INotifyDataErrorInfo, INotifyPropertyChanged
	{
		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
		public event PropertyChangedEventHandler? PropertyChanged;

		private readonly Dictionary<string, IReadOnlyCollection<string>> _errors = [];

		[JsonIgnore]
		public bool HasErrors => _errors.Count > 0;

		private void OnErrorsChanged(string propertyName)
		{
			ErrorsChanged?.Invoke(this, new(propertyName));
			OnPropertyChanged(HasErrors, nameof(HasErrors));
		}

		protected internal void ValidateProperty<TValue>(TValue value, [CallerArgumentExpression(nameof(value))] string? propertyName = null)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

			if (_errors.Remove(propertyName))
			{
				OnErrorsChanged(propertyName);
			}

			List<ValidationResult> validationResults = [];

			if (!Validator.TryValidateProperty(value, new(this)
			{
				MemberName = propertyName
			}, validationResults))
			{
				_errors.Add(propertyName, validationResults.ConvertAll(static result => result.ErrorMessage ?? string.Empty).AsReadOnly());

				OnErrorsChanged(propertyName);
			}
		}

		protected internal void OnPropertyChanged<TValue>(TValue value, [CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new(propertyName));

			if (!string.IsNullOrWhiteSpace(propertyName))
			{
				OnModified();

				ValidateProperty(value, propertyName);
			}
		}

		public IEnumerable GetErrors(string? propertyName)
		{
			return propertyName is null
			? _errors.SelectMany(static pair => pair.Value).ToArray().AsReadOnly()
			: _errors.TryGetValue(propertyName, out IReadOnlyCollection<string>? value) ? value : [];
		}
	}
}
