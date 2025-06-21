namespace WinUIApp.Validation
{
	internal static class ValidationExtensions
	{
		private static void CheckForErrors<TModel>(Control control, string bindingPath, string? propertyName)
			where TModel : notnull, INotifyDataErrorInfo
		{
			ArgumentNullException.ThrowIfNull(control, nameof(control));
			ArgumentException.ThrowIfNullOrWhiteSpace(bindingPath, nameof(bindingPath));

			if (string.IsNullOrWhiteSpace(propertyName) || bindingPath != propertyName)
			{
				return;
			}

			if (control.DataContext is TModel model && control.FindFirstVisualChild<TextBlock>("ErrorContent") is TextBlock textBlock && model.GetErrors(propertyName) is IReadOnlyCollection<string> errors)
			{
				_ = VisualStateManager.GoToState(control, errors.Count > 0 ? "Invalid" : "Valid", true);

				textBlock.Text = string.Join(Environment.NewLine, errors);
			}
		}

		public static void LoadValidation<TModel>(this Control control)
			where TModel : notnull, INotifyDataErrorInfo
		{
			ArgumentNullException.ThrowIfNull(control, nameof(control));

			if (control is ComboBox comboBox && comboBox.FindFirstVisualChild<Button>("DropDownButton") is Button button)
			{
				button.Click += (_, _) => comboBox.IsDropDownOpen = !comboBox.IsDropDownOpen;
			}

			if (control.DataContext is TModel model)
			{
				void OnLayoutUpdated(object? sender, object e)
				{
					if (control.GetBindingPath() is string bindingPath)
					{
						model.ErrorsChanged += (_, args) => CheckForErrors<TModel>(control, bindingPath, args.PropertyName);

						CheckForErrors<TModel>(control, bindingPath, bindingPath);
					}

					control.LayoutUpdated -= OnLayoutUpdated;
				}

				control.LayoutUpdated += OnLayoutUpdated;
			}
		}
	}
}
