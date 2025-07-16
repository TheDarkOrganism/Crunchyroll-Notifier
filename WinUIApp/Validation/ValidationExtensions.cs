namespace WinUIApp.Validation
{
	internal static class ValidationExtensions
	{
		private static void CheckForErrors<TModel>(Control control, TModel model, string bindingPath, string? propertyName)
			where TModel : notnull, INotifyDataErrorInfo
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(bindingPath, nameof(bindingPath));

			if (string.IsNullOrWhiteSpace(propertyName) || bindingPath != propertyName)
			{
				return;
			}

			if (control.FindFirstVisualChild<TextBlock>("ErrorContent") is TextBlock textBlock && model.GetErrors(propertyName) is IReadOnlyCollection<string> errors)
			{
				_ = VisualStateManager.GoToState(control, errors.Count > 0 ? "Invalid" : "Valid", true);

				textBlock.Text = string.Join(Environment.NewLine, errors);
			}
		}

		private static void SetupControl(Control control)
		{
			ArgumentNullException.ThrowIfNull(control, nameof(control));

			if (control is ComboBox comboBox && comboBox.FindFirstVisualChild<Button>("DropDownButton") is Button button)
			{
				button.Click += (_, _) => comboBox.IsDropDownOpen = !comboBox.IsDropDownOpen;
			}
		}

		private static void SetupBinding<TModel>(Control control, TModel model)
			where TModel: notnull, INotifyDataErrorInfo
		{
			void OnLayoutUpdated(object? sender, object e)
			{
				if (control.GetBindingPath() is string bindingPath)
				{
					model.ErrorsChanged += (_, args) => CheckForErrors(control, model, bindingPath, args.PropertyName);

					CheckForErrors(control, model, bindingPath, bindingPath);
				}

				control.LayoutUpdated -= OnLayoutUpdated;
			}

			control.LayoutUpdated += OnLayoutUpdated;
		}

		public static void LoadValidation<TModel>(this Control control)
			where TModel : notnull, INotifyDataErrorInfo
		{
			SetupControl(control);

			if (control.DataContext is TModel model)
			{
				SetupBinding(control, model);
			}
		}

		public static void LoadValidation<TModel>(this Control control, TModel model)
			where TModel : notnull, INotifyDataErrorInfo
		{
			ArgumentNullException.ThrowIfNull(model, nameof(model));

			SetupControl(control);

			SetupBinding(control, model);
		}
	}
}
