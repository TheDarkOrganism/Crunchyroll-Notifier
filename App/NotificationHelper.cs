using CommunityToolkit.WinUI.Notifications;

namespace App
{
	/// <summary>
	/// A class used to show and handle notifications.
	/// </summary>
	internal sealed class NotificationHelper
	{
		/// <summary>
		/// The key for a button to open a file.
		/// </summary>
		public const string openKey = "Open";

		/// <summary>
		/// The value used to open the config file.
		/// </summary>
		public const string configValue = "Config";

		/// <summary>
		/// The value used to open the default config.
		/// </summary>
		public const string defaultConfigValue = "Default Config";

		private readonly CancellationToken _exitToken;

		/// <summary>
		/// Creates a new instance of <see cref="NotificationHelper"/>
		/// with the <paramref name="exitToken"/>.
		/// </summary>
		/// <param name="exitToken">
		/// The token used to exit the program.
		/// </param>
		public NotificationHelper(CancellationToken exitToken)
		{
			_exitToken = exitToken;

			ToastNotificationManagerCompat.OnActivated += async toastArgs =>
			{
				ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

				if (args.TryGetValue(openKey, out string value))
				{
					switch (value)
					{
						case configValue:
							await ConfigManager.OpenConfigAsync(exitToken);
							break;
						case defaultConfigValue:
							await ConfigManager.OpenDefaultConfigAsync(exitToken);
							break;
						default:
							break;
					}
				}
			};
		}

		/// <summary>
		/// Shows a notification with the
		/// <paramref name="title"/> and
		/// <paramref name="message"/>
		/// using the <paramref name="modifier"/>
		/// to modify the <see cref="ToastContentBuilder"/>.
		/// </summary>
		/// <param name="title">
		/// The title of the notification.
		/// </param>
		/// <param name="message">
		/// The message under the <paramref name="title"/>
		/// for the notifation.
		/// </param>
		/// <param name="modifier">
		/// The delegate used to modify
		/// the <see cref="ToastContentBuilder"/>.
		/// </param>
		private void ShowNotification(string title, string message, Action<ToastContentBuilder> modifier)
		{
			ToastContentBuilder builder = new ToastContentBuilder().AddText(title).AddText(message);

			modifier(builder);

			if (!_exitToken.IsCancellationRequested)
			{
				builder.Show();
			}
		}

		/// <summary>
		/// Shows a notification
		/// with the <paramref name="title"/>
		/// and <paramref name="message"/>.
		/// </summary>
		/// <param name="title">
		/// The title of the notification.
		/// </param>
		/// <param name="message">
		/// The message under the <paramref name="title"/>
		/// for the notifation.
		/// </param>
		public void Notify(string title, string message)
		{
			ShowNotification(title, message, _ => { });
		}

		/// <summary>
		/// Shows a notification
		/// with the <paramref name="title"/>
		/// and <paramref name="message"/>
		/// with the <paramref name="button"/>.
		/// </summary>
		/// <param name="title">
		/// The title of the notification.
		/// </param>
		/// <param name="message">
		/// The message under the <paramref name="title"/>
		/// for the notifation.
		/// </param>
		/// <param name="button">
		/// The button under the <paramref name="message"/>.
		/// </param>
		private void Notify(string title, string message, ToastButton button)
		{
			ShowNotification(title, message, builder => builder.AddButton(button));
		}

		/// <summary>
		/// Shows a notification
		/// with the <paramref name="title"/>
		/// and <paramref name="message"/>
		/// and <paramref name="buttonText"/>
		/// with the <paramref name="buttonKey"/>
		/// and the <paramref name="buttonValue"/>.
		/// </summary>
		/// <param name="title">
		/// The title of the notification.
		/// </param>
		/// <param name="message">
		/// The message under the <paramref name="title"/>
		/// for the notifation.
		/// </param>
		/// <param name="buttonText">
		/// The text of the button.
		/// </param>
		/// <param name="buttonKey">
		/// The key of the button event.
		/// </param>
		/// <param name="buttonValue">
		/// The value passed to a button event.
		/// </param>
		public void Notify(string title, string message, string buttonText, string buttonKey, string buttonValue)
		{
			Notify(title, message, new ToastButton(buttonText, string.Join('=', buttonKey, buttonValue)));
		}

		/// <summary>
		/// Shows a notification
		/// with the <paramref name="title"/>
		/// and <paramref name="message"/>
		/// and <paramref name="buttonText"/>
		/// and the <paramref name="arguments"/>.
		/// </summary>
		/// <param name="title">
		/// The title of the notification.
		/// </param>
		/// <param name="message">
		/// The message under the <paramref name="title"/>
		/// for the notifation.
		/// </param>
		/// <param name="buttonText">
		/// The text of the button.
		/// </param>
		/// <param name="arguments">
		/// The arguments passed to the event when
		/// the button is pressed.
		/// </param>
		public void Notify(string title, string message, string buttonText, string arguments)
		{
			Notify(title, message, new ToastButton(buttonText, arguments)
			{
				ActivationType = ToastActivationType.Protocol
			});
		}
	}
}
