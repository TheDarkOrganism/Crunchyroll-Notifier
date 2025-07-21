namespace WinUIApp.Options
{
	[JsonSerializable(typeof(IConfigModel))]
	[JsonSerializable(typeof(ILastUpdateModel))]
	[JsonSerializable(typeof(Dictionary<string, IConfigModel>))]
	[JsonSerializable(typeof(Dictionary<string, ILastUpdateModel>))]
	internal sealed partial class OptionsSerializerContext : JsonSerializerContext { }
}
