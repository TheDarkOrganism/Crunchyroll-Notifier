using Serilog;
using Serilog.Extensions.Logging;

namespace WinUIApp.Providers
{
	internal static class LoggerProvider
	{
		public static ILogger<T> GetLogger<T>()
		{
			using SerilogLoggerFactory loggerFactory = new(Log.Logger);

			return loggerFactory.CreateLogger<T>();
		}
	}
}
