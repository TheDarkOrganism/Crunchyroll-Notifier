namespace WinUIApp.Services
{
	internal sealed class MainService(ISavableJsonOptions<IConfigModel> configOptions, IOptions<ILastUpdateModel> lastUpdateOptions, NotificationHelper notificationHelper) : IHostedService
	{
		public async Task StartAsync(CancellationToken cancellationToken)
		{
			IConfigModel configModel = configOptions.Value;

			if (!configModel.Modified && configModel.ShowFirstRun && !lastUpdateOptions.Value.Modified)
			{
				configModel.ShowFirstRun = false;

				await configOptions.SaveAsync(cancellationToken);

				notificationHelper.Notify("Welcome", "You can find the settings in the system tray by clicking the icon.");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
