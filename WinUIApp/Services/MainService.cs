namespace WinUIApp.Services
{
	internal sealed class MainService(ISavableJsonOptions<ConfigModel> configOptions, NotificationHelper notificationHelper) : IHostedService
	{
		public async Task StartAsync(CancellationToken cancellationToken)
		{
			ConfigModel configModel = configOptions.Value;

			if (!configModel.Modified && configModel.ShowFirstRun)
			{
				configModel.ShowFirstRun = false;

				await configOptions.SaveAsync();

				notificationHelper.Notify("Welcome", "You can find the settings under the system tray and by clicking on the icon.");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
