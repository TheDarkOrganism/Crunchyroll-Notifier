namespace WinUIApp.Logging
{
	internal sealed class FileLogger : ILogger
	{
		private readonly string _file;

		private readonly LogLevel _minLevel;

		private readonly string _catagory;

		private readonly HostBuilderContext _context;

		private readonly Lock _lock = new();

		public FileLogger(string file, LogLevel minLevel, string catagoryName, HostBuilderContext context)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));
			ArgumentException.ThrowIfNullOrWhiteSpace(catagoryName, nameof(catagoryName));
			ArgumentNullException.ThrowIfNull(context, nameof(context));

			_file = file;
			_minLevel = minLevel;
			_catagory = catagoryName;

			_context = context;
		}

		public IDisposable? BeginScope<TState>(TState state)
			where TState : notnull
		{
			return null;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return logLevel >= _minLevel && bool.TryParse(_context.Configuration.GetSection("Config").GetSection("useLogging").Value, out bool useLogging) && useLogging;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			if (formatter is null || !IsEnabled(logLevel))
			{
				return;
			}

			lock (_lock)
			{
				try
				{
					File.AppendAllText(Path.Combine(_context.HostingEnvironment.ContentRootPath, _file), $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] [{_catagory}] {formatter(state, exception)}{(exception is null ? string.Empty : $" {exception}")} {Environment.NewLine}");
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
		}
	}
}
