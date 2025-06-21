namespace WinUIApp.Options
{
	internal sealed class SavableJsonOptions<TOptions> : ISavableJsonOptions<TOptions>
		where TOptions : ModelBase
	{
		private static readonly JsonWriterOptions _jsonWriterOptions = new()
		{
			IndentCharacter = '\t',
			Indented = true,
			IndentSize = 1
		};

		private readonly string _file;
		private readonly string _section;
		private readonly IFileProvider _fileProvider;
		private readonly IOptions<TOptions> _options;

		public SavableJsonOptions(string file, string section, IFileProvider fileProvider, IOptions<TOptions> options)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));
			ArgumentException.ThrowIfNullOrWhiteSpace(section, nameof(section));
			ArgumentNullException.ThrowIfNull(fileProvider, nameof(fileProvider));
			ArgumentNullException.ThrowIfNull(options, nameof(options));

			_file = file;
			_section = section;
			_fileProvider = fileProvider;
			_options = options;
		}

		public TOptions Value => _options.Value;

		private bool TryGetFileStream([NotNullWhen(true)] out FileStream? fileStream)
		{
			try
			{
				IFileInfo fileInfo = _fileProvider.GetFileInfo(_file);

				fileStream = File.Create(fileInfo.PhysicalPath ?? fileInfo.Name);

				return true;
			}
			catch
			{
				fileStream = null;
				return false;
			}
		}

		private void Write(Utf8JsonWriter utf8JsonWriter)
		{
			JsonSerializer.Serialize(utf8JsonWriter, new Dictionary<string, TOptions>
			{
				{ _section, Value }
			});
		}

		public void Save()
		{
			if (Value.Modified && TryGetFileStream(out FileStream? fileStream))
			{
				try
				{
					using Utf8JsonWriter utf8JsonWriter = new(fileStream, _jsonWriterOptions);

					Write(utf8JsonWriter);

					Value.MarkUnmodified();
				}
				finally
				{
					fileStream.Dispose();
				}
			}
		}

		public async Task SaveAsync()
		{
			if (Value.Modified && TryGetFileStream(out FileStream? fileStream))
			{
				try
				{
					await using Utf8JsonWriter utf8JsonWriter = new(fileStream, _jsonWriterOptions);

					Write(utf8JsonWriter);

					Value.MarkUnmodified();
				}
				finally
				{
					await fileStream.DisposeAsync();
				}
			}
		}
	}
}
