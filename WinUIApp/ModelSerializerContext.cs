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
	}
}
