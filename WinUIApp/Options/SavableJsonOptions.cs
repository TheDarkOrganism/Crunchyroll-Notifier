namespace WinUIApp.Options
{
	internal sealed partial class SavableJsonOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TIOptions> : ISavableJsonOptions<TIOptions>
		where TIOptions : class, IModelBase
	{
		private static readonly ModelSerializerContext _serializerContext = ModelSerializerContext.Writing;

		private static readonly Type ValueType = typeof(Dictionary<string, TIOptions>);

		private readonly IFileModel<TIOptions> _fileModel;
		private readonly string _file;
		private readonly IHostEnvironment _environment;
		private readonly IOptions<TIOptions> _options;
		private readonly IConfigurationRoot _configurationRoot;
		private readonly ILogger<SavableJsonOptions<TIOptions>> _logger;

		public SavableJsonOptions(IFileModel<TIOptions> fileModel, HostBuilderContext context, IOptions<TIOptions> options, ILogger<SavableJsonOptions<TIOptions>> logger)
		{
			ArgumentNullException.ThrowIfNull(fileModel, nameof(fileModel));
			ArgumentNullException.ThrowIfNull(context, nameof(context));
			ArgumentNullException.ThrowIfNull(options, nameof(options));
			ArgumentNullException.ThrowIfNull(logger, nameof(logger));

			_fileModel = fileModel;
			_file = fileModel.File;
			_environment = context.HostingEnvironment;
			_options = options;
			_configurationRoot = (IConfigurationRoot)context.Configuration;
			_logger = logger;
		}

		public TIOptions Value => _options.Value;

		private bool TryGetFileStream([NotNullWhen(true)] out FileStream? fileStream)
		{
			try
			{
				IFileInfo fileInfo = _environment.ContentRootFileProvider.GetFileInfo(_file);

				fileStream = File.Create(fileInfo.PhysicalPath ?? Path.Combine(_environment.ContentRootPath, fileInfo.Name));

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get to {FileStream} for {File}.", nameof(FileStream), _file);

				fileStream = null;
				return false;
			}
		}

		private Dictionary<string, TIOptions> GetValue()
		{
			return new()
			{
				{ _fileModel.ConfigurationSection, Value }
			};
		}

		private void HandleReload()
		{
			if (Value.ReloadConfiguration)
			{
				_configurationRoot.Reload();
			}
		}

		public void Save()
		{
			_fileModel.Wait(() =>
			{
				if (Value.Modified && TryGetFileStream(out FileStream? fileStream))
				{
					try
					{
						JsonSerializer.Serialize(fileStream, GetValue(), ValueType, _serializerContext);

						HandleReload();

						Value.MarkUnmodified();
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Failed to serialize {FileStream} for {File}", nameof(FileStream), _file);
					}
					finally
					{
						fileStream.Dispose();
					}
				}
			}, _logger);
		}

		public async Task SaveAsync(CancellationToken cancellationToken)
		{
			await _fileModel.WaitAsync(async token =>
			{
				if (Value.Modified && TryGetFileStream(out FileStream? fileStream))
				{
					try
					{
						await JsonSerializer.SerializeAsync(fileStream, GetValue(), ValueType, _serializerContext, token);

						HandleReload();

						Value.MarkUnmodified();
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Failed to serialize {FileStream} for {File}", nameof(FileStream), _file);
					}
					finally
					{
						await fileStream.DisposeAsync();
					}
				}
			}, _logger, cancellationToken);
		}
	}
}
