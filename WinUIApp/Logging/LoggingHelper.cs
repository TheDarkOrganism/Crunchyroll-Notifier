namespace WinUIApp.Logging
{
	internal static class LoggingHelper
	{
		private static FileLoggerProvider? _loggerProvider;

		private const LogLevel _minLevel =
#if DEBUG
			LogLevel.Trace;
#else
			LogLevel.Information;
#endif

		public static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
		{
			_loggerProvider ??= new("Logs.txt", _minLevel, context);

			_ = builder.SetMinimumLevel(_minLevel).AddDebug().AddProvider(_loggerProvider);
		}
	}
}
