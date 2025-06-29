namespace WinUIApp.Services
{
	internal sealed class ResourceRecoveryService : IHostedService
	{
		private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

		private readonly string _appDirectory;

		private readonly FileSystemWatcher _watcher;

		public ResourceRecoveryService(string file, string appDirectory)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));
			ArgumentException.ThrowIfNullOrWhiteSpace(appDirectory, nameof(appDirectory));

			if (!Directory.Exists(appDirectory))
			{
				throw new DirectoryNotFoundException();
			}

			_watcher = new(appDirectory)
			{
				EnableRaisingEvents = true
			};

			if (!File.Exists(file))
			{
				ResourceHelper.RestoreFile(file);
			}

			_appDirectory = appDirectory;

			_watcher.Filters.Add(file);
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_watcher.Deleted += (_, args) => ResourceHelper.RestoreFile(args.FullPath);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_watcher.Dispose();

			return Task.CompletedTask;
		}
	}
}
