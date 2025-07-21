namespace WinUIApp.Services
{
	internal sealed class ResourceRecoveryService<TIModel> : IHostedService
		where TIModel : class, IModelBase
	{
		private readonly IFileModel<TIModel> _fileModel;
		private readonly FileSystemWatcher _watcher;

		public ResourceRecoveryService(IFileModel<TIModel> fileModel, IHostEnvironment hostEnvironment)
		{
			ArgumentNullException.ThrowIfNull(fileModel, nameof(fileModel));
			ArgumentNullException.ThrowIfNull(hostEnvironment, nameof(hostEnvironment));

			_fileModel = fileModel;

			IFileInfo fileInfo = hostEnvironment.ContentRootFileProvider.GetFileInfo(fileModel.File);

			if (!fileInfo.Exists)
			{
				ResourceHelper.RestoreFile(fileModel);
			}

			_watcher = new FileSystemWatcher(hostEnvironment.ContentRootPath)
			{
				EnableRaisingEvents = true
			};

			_watcher.Filters.Add(fileInfo.Name);
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_watcher.Deleted += (_, _) => ResourceHelper.RestoreFile(_fileModel);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_watcher.Dispose();

			return Task.CompletedTask;
		}
	}
}
