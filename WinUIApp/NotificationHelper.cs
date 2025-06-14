using Microsoft.Windows.AppNotifications.Builder;

namespace WinUIApp
{
	public sealed class NotificationHelper
	{
		private readonly AppNotificationManager _notificationManager;

		public NotificationHelper(AppNotificationManager notificationManager)
		{
			_notificationManager = notificationManager;

			notificationManager.Register();
		}

		private void ShowNotification(string title, string message, Action<AppNotificationBuilder> modifier)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
			ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

			AppNotificationBuilder builder = new AppNotificationBuilder().AddText(title).AddText(message);

			modifier(builder);

			_notificationManager.Show(builder.BuildNotification());
		}

		internal void Notify(string title, string message)
		{
			ShowNotification(title, message, static _ => { });
		}

		private void ShowNotification(string title, string message, string buttonText, Func<AppNotificationButton, AppNotificationButton> modifier)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(buttonText, nameof(buttonText));

			ShowNotification(title, message, builder => builder.AddButton(modifier(new(buttonText))));
		}

		internal void Notify(string title, string message, string buttonText)
		{
			ShowNotification(title, message, buttonText, static button => button);
		}

		internal void Notify(string title, string message, string buttonText, Uri protocolUri)
		{
			ShowNotification(title, message, buttonText, button => button.SetInvokeUri(protocolUri));
		}
	}
}
