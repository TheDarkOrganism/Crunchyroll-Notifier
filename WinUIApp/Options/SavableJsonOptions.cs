namespace WinUIApp.Options
{
	internal sealed partial class SavableJsonOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TIOptions> : ISavableJsonOptions<TIOptions>
		where TIOptions : class, IModelBase
	{
		private static readonly OptionsSerializerContext _serializerContext = new(new()
		{
			WriteIndented = true,
			IndentCharacter = '\t',
			IndentSize = 1
		});

		private static readonly Type ValueType = typeof(Dictionary<string, TIOptions>);

		private readonly IFileModel<TIOptions> _fileModel;
		private readonly string _file;
		private readonly IFileProvider _fileProvider;
		private readonly IOptions<TIOptions> _options;
		private readonly IConfigurationRoot _configurationRoot;
		private readonly ILogger<SavableJsonOptions<TIOptions>> _logger;

		public SavableJsonOptions(IFileModel<TIOptions> fileModel, IHostEnvironment hostEnvironment, IOptions<TIOptions> options, IConfiguration configuration, ILogger<SavableJsonOptions<TIOptions>> logger)
		{
			ArgumentNullException.ThrowIfNull(fileModel, nameof(fileModel));
			ArgumentNullException.ThrowIfNull(hostEnvironment, nameof(hostEnvironment));
			ArgumentNullException.ThrowIfNull(options, nameof(options));
			ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
			ArgumentNullException.ThrowIfNull(logger, nameof(logger));

			_fileModel = fileModel;
			_file = fileModel.File;
			_fileProvider = hostEnvironment.ContentRootFileProvider;
			_options = options;
			_configurationRoot = (IConfigurationRoot)configuration;
			_logger = logger;
		}

		public TIOptions Value => _options.Value;

		private bool TryGetFileStream([NotNullWhen(true)] out FileStream? fileStream)
		{
			try
			{
				IFileInfo fileInfo = _fileProvider.GetFileInfo(_file);

				fileStream = File.Create(fileInfo.PhysicalPath ?? fileInfo.Name);

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
			lock (_fileModel.Lock)
			{
				if (Value.Modified && TryGetFileStream(out FileStream? fileStream))
				{
					try
					{
						JsonSerializer.Serialize(fileStream, GetValue(), ValueType, _serializerContext);

						fileStream.Dispose();

						HandleReload();

						Value.MarkUnmodified();
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Failed to serialize {FileStream} for {File}", nameof(FileStream), _file);

						fileStream.Dispose();
					}
				}
			}
		}

		public async Task SaveAsync()
		{
			if (Value.Modified && TryGetFileStream(out FileStream? fileStream))
			{
				try
				{
					await JsonSerializer.SerializeAsync(fileStream, GetValue(), ValueType, _serializerContext);

					await fileStream.DisposeAsync();

					HandleReload();

					Value.MarkUnmodified();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to serialize {FileStream} for {File}", nameof(FileStream), _file);

					await fileStream.DisposeAsync();
				}
			}
		}
	}
}
