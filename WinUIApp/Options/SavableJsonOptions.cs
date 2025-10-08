namespace WinUIApp.Options
{
	internal sealed partial class SavableJsonOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TIOptions> : ISavableJsonOptions<TIOptions>
		where TIOptions : class, IModelBase
	{
		private static readonly ModelSerializerContext _serializerContext = ModelSerializerContext.Writing;

		private static readonly JsonDocumentOptions _jsonDocumentOptions = ModelSerializerContext.DocumentOptions;

		private static readonly JsonSerializerOptions _jsonSerializerOptions = _serializerContext.Options;

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
		private readonly ILogger<SavableJsonOptions<TIOptions>> _logger;

		public SavableJsonOptions(IFileModel<TIOptions> fileModel, IHostEnvironment environment, IOptions<TIOptions> options, ILogger<SavableJsonOptions<TIOptions>> logger)
		{
			ArgumentNullException.ThrowIfNull(fileModel, nameof(fileModel));
			ArgumentNullException.ThrowIfNull(environment, nameof(environment));
			ArgumentNullException.ThrowIfNull(options, nameof(options));
			ArgumentNullException.ThrowIfNull(logger, nameof(logger));

			_fileModel = fileModel;
			_file = fileModel.File;
			_environment = environment;
			_options = options;
			_logger = logger;
		}

		public TIOptions Value => _options.Value;

		private bool CanSave()
		{
			if (!Value.Modified)
			{
				_logger.LogNotModified(_file, typeof(TIOptions));

				return false;
			}

			if (Value is INotifyDataErrorInfo errorInfo && errorInfo.HasErrors)
			{
				_logger.LogHasValidationErrors(_file, typeof(TIOptions));

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
				_logger.LogFailGetFileStream(ex, _file);

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
				_logger.LogInvalidJsonConfigiurationObject(_file);
			}

			utf8JsonWriter.WriteEndObject();
		}

		public void Save()
		{
			_fileModel.Wait(() =>
			{
				if (CanSave() && TryGetFileStream(out FileStream? fileStream))
				{
					JsonDocument jsonDocument;

					try
					{
						jsonDocument = JsonDocument.Parse(fileStream, _jsonDocumentOptions);
					}
					catch (Exception ex)
					{
						_logger.LogInvalidJsonConfigiurationDocument(ex, _file);

						using Stream stream = ResourceHelper.GetResource(_file).CreateReadStream();

						jsonDocument = JsonDocument.Parse(stream, _jsonDocumentOptions);
					}

					try
					{
						using Utf8JsonWriter utf8JsonWriter = new(fileStream, _jsonWriterOptions);

						HandleSave(fileStream, utf8JsonWriter, jsonDocument);

						Value.MarkUnmodified();
					}
					catch (Exception ex)
					{
						_logger.LogFailedFileStreamSerialization(ex, _file);
					}
					finally
					{
						jsonDocument.Dispose();
						fileStream.Dispose();
					}
				}
			}, _logger);
		}

		public async Task SaveAsync(CancellationToken cancellationToken)
		{
			await _fileModel.WaitAsync(async token =>
			{
				if (CanSave() && TryGetFileStream(out FileStream? fileStream))
				{
					JsonDocument jsonDocument;

					try
					{
						jsonDocument = await JsonDocument.ParseAsync(fileStream, _jsonDocumentOptions, token);
					}
					catch (Exception ex)
					{
						_logger.LogInvalidJsonConfigiurationDocument(ex, _file);

						await using Stream stream = ResourceHelper.GetResource(_file).CreateReadStream();

						jsonDocument = await JsonDocument.ParseAsync(stream, _jsonDocumentOptions, token);
					}

					try
					{
						await using Utf8JsonWriter utf8JsonWriter = new(fileStream, _jsonWriterOptions);

						HandleSave(fileStream, utf8JsonWriter, jsonDocument);

						Value.MarkUnmodified();
					}
					catch (Exception ex)
					{
						_logger.LogFailedFileStreamSerialization(ex, _file);
					}
					finally
					{
						jsonDocument.Dispose();
						await fileStream.DisposeAsync();
					}
				}
			}, _logger, cancellationToken);
		}
	}
}
