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

		private bool CanSave()
		{
			if (!Value.Modified)
			{
				_logger.LogDebug("{File} can't be saved due to the {Model} not being modified.", _file, nameof(TIOptions));

				return false;
			}

			if (Value is INotifyDataErrorInfo errorInfo && errorInfo.HasErrors)
			{
				_logger.LogDebug("{File} can't be saved due to the {Model} having validation errors.", _file, nameof(TIOptions));

				return false;
			}

			return true;
		}

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

				Value.MarkReloaded();
			}
		}

		public void Save()
		{
			bool saved = false;

			_fileModel.Wait(() =>
			{
				if (CanSave() && TryGetFileStream(out FileStream? fileStream))
				{
					try
					{
						JsonSerializer.Serialize(fileStream, GetValue(), ValueType, _serializerContext);

						Value.MarkUnmodified();

						saved = true;
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

			if (saved)
			{
				HandleReload();
			}
		}

		public async Task SaveAsync(CancellationToken cancellationToken)
		{
			bool saved = false;

			await _fileModel.WaitAsync(async token =>
			{
				if (CanSave() && TryGetFileStream(out FileStream? fileStream))
				{
					try
					{
						await JsonSerializer.SerializeAsync(fileStream, GetValue(), ValueType, _serializerContext, token);

						Value.MarkUnmodified();

						saved = true;
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

			if (saved)
			{
				HandleReload();
			}
		}
	}
}
