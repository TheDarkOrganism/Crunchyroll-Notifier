namespace WinUIApp.Converters
{
	internal sealed class TimeSpanConverter : JsonConverter<TimeSpan>
	{
		public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return reader.TokenType == JsonTokenType.String && TimeSpan.TryParse(reader.GetString(), out TimeSpan result)
				? result
				: reader.TokenType == JsonTokenType.Number ? TimeSpan.FromSeconds(reader.GetDouble()) : TimeSpan.Zero;
		}

		public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString());
		}
	}
}
