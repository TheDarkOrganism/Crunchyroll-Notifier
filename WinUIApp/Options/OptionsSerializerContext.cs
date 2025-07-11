namespace WinUIApp.Options
{
	[JsonSerializable(typeof(ConfigModel))]
	[JsonSerializable(typeof(LastUpdateModel))]
	[JsonSerializable(typeof(Dictionary<string, ConfigModel>))]
	[JsonSerializable(typeof(Dictionary<string, LastUpdateModel>))]
	internal sealed partial class OptionsSerializerContext : JsonSerializerContext { }
}
