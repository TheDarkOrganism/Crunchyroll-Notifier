namespace WinUIApp.Services
{
	internal sealed class ResourceRecoveryService : IHostedService
	{
		private readonly FileSystemWatcher _watcher;

		public ResourceRecoveryService(string file, IHostEnvironment hostEnvironment)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));
			ArgumentNullException.ThrowIfNull(hostEnvironment, nameof(hostEnvironment));

			IFileInfo fileInfo = hostEnvironment.ContentRootFileProvider.GetFileInfo(file);

			string name = fileInfo.Name;

			if (!fileInfo.Exists)
			{
				ResourceHelper.RestoreFile(name);
			}

			_watcher = new FileSystemWatcher(hostEnvironment.ContentRootPath)
			{
				EnableRaisingEvents = true
			};

			_watcher.Filters.Add(name);
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_watcher.Deleted += static (_, args) => ResourceHelper.RestoreFile(args.FullPath);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_watcher.Dispose();

			return Task.CompletedTask;
		}
	}
}
