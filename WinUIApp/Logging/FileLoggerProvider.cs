using System.Collections.Concurrent;

namespace WinUIApp.Logging
{
	internal sealed partial class FileLoggerProvider(string file, LogLevel minLevel, HostBuilderContext context) : ILoggerProvider
	{
		private readonly ConcurrentDictionary<string, FileLogger> _loggers = [];

		public ILogger CreateLogger(string categoryName)
		{
			return _loggers.TryGetValue(categoryName, out FileLogger? value) ? value : _loggers.GetOrAdd(categoryName, category => new(file, minLevel, category, context));
		}

		public void Dispose() { }
	}
}
