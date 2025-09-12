using Microsoft.UI.Windowing;
using WinUIApp.Dialogs;

namespace WinUIApp
{
	internal sealed partial class Settings : Window
	{
		private readonly ISavableJsonOptions<IConfigModel> _options;

		public Settings(ISavableJsonOptions<IConfigModel> options)
		{
			_options = options;

			InitializeComponent();

			AppWindow.SetIcon("Crunchyroll-Notifier.ico");
			AppWindow.Resize(new(463, AppWindow.Size.Height));

#if !DEBUG
			if (AppWindow.Presenter is OverlappedPresenter presenter)
			{
				presenter.IsMaximizable = false;
				presenter.IsResizable = false;
			}
#endif
		}

		private static async ValueTask AskForInput<TModel>(ListView target, UserActionType userAction, string name, int maxlength)
			where TModel : notnull, TextInputModelBase<TModel>, new()
		{
			TModel model = new();

			TextInputDialog textInputDialog = new(userAction, name, maxlength, model)
			{
				XamlRoot = target.XamlRoot
			};

			if (await textInputDialog.ShowAsync(ContentDialogPlacement.InPlace) == ContentDialogResult.Primary && target.ItemsSource is IList<string> values && !model.HasErrors)
			{
				string text = model.Text;

				switch (userAction)
				{
					case UserActionType.add:
						values.Add(text);
						break;
					case UserActionType.remove:
						_ = values.Remove(text);
						break;
					default:
						throw new NotImplementedException();
				}
			}
		}

		private async ValueTask AskForDubInput(UserActionType userAction)
		{
			await AskForInput<DubTextInputModel>(DubsValues, userAction, nameof(ConfigModel.Dubs), 40);
		}

		private async void DubsAddButton_Click(object sender, RoutedEventArgs e)
		{
			await AskForDubInput(UserActionType.add);
		}

		private async void DubsRemoveButton_Click(object sender, RoutedEventArgs e)
		{
			await AskForDubInput(UserActionType.remove);
		}

		private async ValueTask AskForNameInput(UserActionType userAction)
		{
			await AskForInput<NameTextInputModel>(NamesValues, userAction, nameof(ConfigModel.Names), 60);
		}

		private async void NamesAddButton_Click(object sender, RoutedEventArgs e)
		{
			await AskForNameInput(UserActionType.add);
		}

		private async void NamesRemoveButton_Click(object sender, RoutedEventArgs e)
		{
			await AskForNameInput(UserActionType.remove);
		}

		private void Window_Closed(object sender, WindowEventArgs e)
		{
			if (Visible && AppWindow is not null)
			{
				AppWindow.Hide();

				e.Handled = true;
			}

			_options.Save();
		}

		[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Due to this method being a event handler.")]
		[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Hide the message to remove the CA1822 suppression.")]
		private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
		{
			double value = args.NewValue;

			if (double.IsNaN(value))
			{
				sender.Value = value;

				return;
			}

			double min = sender.Minimum;
			double max = sender.Maximum;

			if (value < min || value > max)
			{
				sender.Value = Math.Clamp(value, min, max);
			}
		}

		private void Control_Loaded(object sender, RoutedEventArgs e)
		{
			if (sender is Control control)
			{
				control.LoadValidation<ConfigModel>();
			}

			if (sender is FrameworkElement element)
			{
				element.Loaded -= Control_Loaded;
			}
		}
	}
}
