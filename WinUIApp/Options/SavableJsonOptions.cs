using System.Text.Json.Serialization.Metadata;

namespace WinUIApp.Options
{
	internal sealed partial class SavableJsonOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions> : ISavableJsonOptions<TOptions>
		where TOptions : ModelBase
	{
		private static readonly OptionsSerializerContext _serializerContext = new(new()
		{
			WriteIndented = true,
			IndentCharacter = '\t',
			IndentSize = 1
		});

		private static readonly Type ValueType = typeof(Dictionary<string, TOptions>);

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

		private Dictionary<string, TOptions> GetValue()
		{
			return new()
			{
				{ _section, Value }
			};
		}

		public void Save()
		{
			if (Value.Modified && TryGetFileStream(out FileStream? fileStream))
			{
				try
				{
					JsonSerializer.Serialize(fileStream, GetValue(), ValueType, _serializerContext);

					Value.MarkUnmodified();
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
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
					await JsonSerializer.SerializeAsync(fileStream, GetValue(), ValueType, _serializerContext);

					Value.MarkUnmodified();
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
				finally
				{
					await fileStream.DisposeAsync();
				}
			}
		}
	}
}
