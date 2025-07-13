namespace WinUIApp.Providers
{
	internal sealed class ValidationConfigurationSource<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : IConfigurationSource
		where T : notnull
	{
		private readonly string _file;
		private readonly IFileProvider _fileProvider;
		private readonly ILogger<ValidationConfigurationProvider<T>> _logger;

		public ValidationConfigurationSource(string file, HostBuilderContext context)
		{
			ArgumentNullException.ThrowIfNull(context, nameof(context));

			_file = file;
			_fileProvider = context.HostingEnvironment.ContentRootFileProvider;

			using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => LoggingHelper.ConfigureLogging(context, builder));

			_logger = loggerFactory.CreateLogger<ValidationConfigurationProvider<T>>();
		}

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new ValidationConfigurationProvider<T>(_file, _fileProvider, _logger);
		}
	}
}
