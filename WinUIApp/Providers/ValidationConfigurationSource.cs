using Serilog;
using Serilog.Extensions.Logging;

namespace WinUIApp.Providers
{
	internal sealed class ValidationConfigurationSource<TModel, TIModel> : IConfigurationSource
		where TModel : ModelBase, TIModel
		where TIModel : class, IModelBase
	{
		private readonly IFileModel<TIModel> _fileModel;
		private readonly IFileProvider _fileProvider;
		private readonly ILogger<ValidationConfigurationProvider<TModel, TIModel>> _logger;

		public ValidationConfigurationSource(IFileModel<TIModel> fileModel, HostBuilderContext context)
		{
			ArgumentNullException.ThrowIfNull(context, nameof(context));

			_fileModel = fileModel;
			_fileProvider = context.HostingEnvironment.ContentRootFileProvider;

			using SerilogLoggerFactory loggerFactory = new(Log.Logger);

			_logger = loggerFactory.CreateLogger<ValidationConfigurationProvider<TModel, TIModel>>();
		}

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new ValidationConfigurationProvider<TModel, TIModel>(_fileModel, _fileProvider, _logger);
		}
	}
}
