namespace WinUIApp.Models
{
	internal interface IFileModel<TIModel> : IDisposable
		where TIModel : class, IModelBase
	{
		string File { get; }

		string ConfigurationSection { get; }

		void Wait<T>(Action action, ILogger<T> logger);

		ValueTask WaitAsync<T>(Func<CancellationToken, Task> func, ILogger<T> logger, CancellationToken cancellationToken);
	}
}
