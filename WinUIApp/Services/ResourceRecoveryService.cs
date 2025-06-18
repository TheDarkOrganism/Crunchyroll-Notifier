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
				RestoreFile(file);
			}

			_appDirectory = appDirectory;

			_watcher.Filters.Add(file);
		}

		private void RestoreFile(string file)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));

			using Stream? stream = _assembly.GetManifestResourceStream(string.Join('.', nameof(WinUIApp), Path.GetRelativePath(_appDirectory, file).Replace(Path.PathSeparator, '.')));
		
			if (stream is not null)
			{
				using FileStream fileStream = File.Create(file);

				stream.CopyTo(fileStream);
			}
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_watcher.Deleted += (_, args) => RestoreFile(args.FullPath);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_watcher.Dispose();

			return Task.CompletedTask;
		}
	}
}
