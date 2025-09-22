namespace WinUIApp.Options
{
	internal sealed partial class SavableJsonOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TIOptions> : ISavableJsonOptions<TIOptions>
		where TIOptions : class, IModelBase
	{
		private static readonly ModelSerializerContext _serializerContext = ModelSerializerContext.Writing;

		private static readonly JsonSerializerOptions _jsonSerializerOptions = _serializerContext.Options;

		private static readonly JsonDocumentOptions _jsonDocumentOptions = new()
		{
			AllowTrailingCommas = _jsonSerializerOptions.AllowTrailingCommas,
			MaxDepth = _jsonSerializerOptions.MaxDepth,
			CommentHandling = _jsonSerializerOptions.ReadCommentHandling
		};

		private static readonly JsonWriterOptions _jsonWriterOptions = new()
		{
			Encoder = _jsonSerializerOptions.Encoder,
			IndentCharacter = _jsonSerializerOptions.IndentCharacter,
			Indented = _jsonSerializerOptions.WriteIndented,
			IndentSize = _jsonSerializerOptions.IndentSize,
			MaxDepth = _jsonSerializerOptions.MaxDepth,
			NewLine = _jsonSerializerOptions.NewLine
		};

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

				fileStream = File.Open(fileInfo.PhysicalPath ?? Path.Combine(_environment.ContentRootPath, fileInfo.Name), FileMode.OpenOrCreate, FileAccess.ReadWrite);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get to {FileStream} for {File}.", nameof(FileStream), _file);

				fileStream = null;
				return false;
			}
		}

		private void HandleSave(FileStream fileStream, Utf8JsonWriter utf8JsonWriter, JsonDocument jsonDocument)
		{
			fileStream.SetLength(0);

			utf8JsonWriter.WriteStartObject();

			string section = _fileModel.ConfigurationSection;

			utf8JsonWriter.WritePropertyName(section);

			JsonSerializer.Serialize(utf8JsonWriter, Value, typeof(TIOptions), _serializerContext);

			JsonElement rootElement = jsonDocument.RootElement;

			if (rootElement.ValueKind == JsonValueKind.Object)
			{
				foreach (JsonProperty jsonProperty in rootElement.EnumerateObject())
				{
					if (!jsonProperty.NameEquals(section))
					{
						jsonProperty.WriteTo(utf8JsonWriter);
					}
				}
			}
			else
			{
				_logger.LogWarning("Configuration for {File} was not a valid JSON object.", _file);
			}

			utf8JsonWriter.WriteEndObject();
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
					JsonDocument jsonDocument;

					try
					{
						jsonDocument = JsonDocument.Parse(fileStream, _jsonDocumentOptions);
					}
					catch
					{
						_logger.LogWarning("Configuration for {File} was not a valid JSON document.", _file);

						using Stream stream = ResourceHelper.GetResource(_file).CreateReadStream();

						jsonDocument = JsonDocument.Parse(stream, _jsonDocumentOptions);
					}

					try
					{
						using Utf8JsonWriter utf8JsonWriter = new(fileStream, _jsonWriterOptions);

						HandleSave(fileStream, utf8JsonWriter, jsonDocument);

						Value.MarkUnmodified();

						saved = true;
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Failed to serialize {FileStream} for {File}", nameof(FileStream), _file);
					}
					finally
					{
						jsonDocument.Dispose();
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
					JsonDocument jsonDocument;

					try
					{
						jsonDocument = await JsonDocument.ParseAsync(fileStream, _jsonDocumentOptions, cancellationToken);
					}
					catch
					{
						_logger.LogWarning("Configuration for {File} was not a valid JSON document.", _file);

						await using Stream stream = ResourceHelper.GetResource(_file).CreateReadStream();

						jsonDocument = await JsonDocument.ParseAsync(stream, _jsonDocumentOptions, cancellationToken);
					}

					try
					{
						await using Utf8JsonWriter utf8JsonWriter = new(fileStream, _jsonWriterOptions);

						HandleSave(fileStream, utf8JsonWriter, jsonDocument);

						Value.MarkUnmodified();

						saved = true;
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Failed to serialize {FileStream} for {File}", nameof(FileStream), _file);
					}
					finally
					{
						jsonDocument.Dispose();
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
