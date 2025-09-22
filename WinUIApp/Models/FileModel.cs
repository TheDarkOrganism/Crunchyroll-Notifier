namespace WinUIApp.Models
{
	internal sealed partial class FileModel<TModel, TIModel> : IFileModel<TIModel>
		where TModel : notnull, ModelBase, TIModel
		where TIModel : class, IModelBase
	{
		private readonly SemaphoreSlim _lock;

		public string File { get; }

		public string ConfigurationSection { get; }

		public FileModel(string file, string section)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));
			ArgumentException.ThrowIfNullOrWhiteSpace(section, nameof(section));

			File = file;

			ConfigurationSection = section;

			_lock = new(1, 1);
		}

		public FileModel(string file) : this(file, Path.GetFileNameWithoutExtension(file)) { }

		private void LogException<T>(Exception ex, ILogger<T> logger)
		{
			switch (ex)
			{
				case ObjectDisposedException:
					logger.LogDebug("The operation was canceled because the application is shutting down.");
					break;
				case OperationCanceledException:
					logger.LogDebug("The operation was canceled.");
					break;
				default:
					logger.LogError(ex, "Failed to wait for the file {File}.", File);
					throw ex;
			}
		}

		public void Wait<T>(Action action, ILogger<T> logger)
		{
			bool lockTaken = false;

			try
			{
				_lock.Wait();

				lockTaken = true;

				action();
			}
			catch (Exception ex)
			{
				LogException(ex, logger);
			}
			finally
			{
				if (lockTaken)
				{
					_lock.Release();
				}
			}
		}

		public async ValueTask WaitAsync<T>(Func<CancellationToken, Task> func, ILogger<T> logger, CancellationToken cancellationToken)
		{
			bool lockTaken = false;

			try
			{
				await _lock.WaitAsync(cancellationToken);

				lockTaken = true;

				await func(cancellationToken);
			}
			catch (Exception ex)
			{
				LogException(ex, logger);
			}
			finally
			{
				if (lockTaken)
				{
					_lock.Release();
				}
			}
		}

		public void Dispose()
		{
			_lock.Dispose();
		}
	}
}
