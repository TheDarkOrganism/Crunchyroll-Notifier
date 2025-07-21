namespace WinUIApp.Providers
{
	internal sealed class ValidationConfigurationSource<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TIModel> : IConfigurationSource
		where TIModel : class, IModelBase
	{
		private readonly IFileModel<TIModel> _fileModel;
		private readonly IFileProvider _fileProvider;
		private readonly ILogger<ValidationConfigurationProvider<TIModel>> _logger;

		public ValidationConfigurationSource(IFileModel<TIModel> fileModel, HostBuilderContext context)
		{
			ArgumentNullException.ThrowIfNull(context, nameof(context));

			_fileModel = fileModel;	
			_fileProvider = context.HostingEnvironment.ContentRootFileProvider;

			using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => LoggingHelper.ConfigureLogging(context, builder));

			_logger = loggerFactory.CreateLogger<ValidationConfigurationProvider<TIModel>>();
		}

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new ValidationConfigurationProvider<TIModel>(_fileModel, _fileProvider, _logger);
		}
	}
}
