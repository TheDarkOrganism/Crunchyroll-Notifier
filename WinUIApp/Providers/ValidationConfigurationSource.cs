namespace WinUIApp.Providers
{
	internal sealed class ValidationConfigurationSource<TModel> : JsonConfigurationSource
		where TModel : notnull, ModelBase
	{
		private readonly ILogger<ValidationConfigurationProvider<TModel>> _logger;
		private readonly string _section;

		public ValidationConfigurationSource(string file, string section, bool optional, bool reloadOnChange)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));

			Path = file;
			Optional = optional;
			ReloadOnChange = reloadOnChange;
			_section = section;

			_logger = LoggerProvider.GetLogger<ValidationConfigurationProvider<TModel>>();
		}

		public override IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			EnsureDefaults(builder);

			return new ValidationConfigurationProvider<TModel>(this, _section, _logger);
		}
	}
}
