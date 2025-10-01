namespace WinUIApp
{
	[JsonSerializable(typeof(ConfigModel), GenerationMode = JsonSourceGenerationMode.Metadata)]
	[JsonSerializable(typeof(IConfigModel))]
	[JsonSerializable(typeof(FeedHostType))]
	[JsonSerializable(typeof(VisibilityType))]
	[JsonSerializable(typeof(LastUpdateModel), GenerationMode = JsonSourceGenerationMode.Metadata)]
	[JsonSerializable(typeof(ILastUpdateModel))]
	[JsonSerializable(typeof(DubTextInputModel), GenerationMode = JsonSourceGenerationMode.Metadata)]
	[JsonSerializable(typeof(NameTextInputModel), GenerationMode = JsonSourceGenerationMode.Metadata)]
	[JsonSourceGenerationOptions(AllowTrailingCommas = true, Converters = [typeof(Converters.InterfaceConverter), typeof(Converters.TimeSpanConverter)], DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, UseStringEnumConverter = true)]
	internal sealed partial class ModelSerializerContext : JsonSerializerContext
	{
		private static JsonDocumentOptions? _documentOptions;

		public static JsonDocumentOptions DocumentOptions
		{
			get
			{
				JsonSerializerOptions options = Default.Options;

				_documentOptions ??= new()
				{
					AllowTrailingCommas = options.AllowTrailingCommas,
					MaxDepth = options.MaxDepth,
					CommentHandling = options.ReadCommentHandling
				};

				return _documentOptions.Value;
			}
		}

		private static ModelSerializerContext? _writing;

		public static ModelSerializerContext Writing
		{
			get
			{
				_writing ??= new(new(Default.Options)
				{
					IndentCharacter = '\t',
					IndentSize = 1,
					WriteIndented = true
				});

				return _writing;
			}
		}

		public string GetJsonName(string name)
		{
			return Options.PropertyNamingPolicy?.ConvertName(name) ?? name;
		}

		public bool TryGetJsonTypeInfo([NotNullWhen(true)] Type? type, [NotNullWhen(true)] out JsonTypeInfo? jsonTypeInfo)
		{
			jsonTypeInfo = null;

			if (type is null)
			{
				return false;
			}

			jsonTypeInfo = GetTypeInfo(type);

			return jsonTypeInfo is not null;
		}
	}
}
