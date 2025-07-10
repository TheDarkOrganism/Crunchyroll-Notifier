
namespace WinUIApp.Converters
{
	internal sealed class EnumConverter<TEnum> : JsonConverter<TEnum>
		where TEnum : struct, Enum
	{
		public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return Enum.TryParse(reader.GetString(), true, out TEnum result) ? result : default;
		}

		public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString());
		}
	}
}
