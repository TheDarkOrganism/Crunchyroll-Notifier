﻿namespace App.Converters
{
	internal sealed class TimeSpanConverter : JsonConverter<TimeSpan>
	{
		public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return TimeSpan.FromSeconds(reader.GetDouble());
		}

		public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(value.TotalSeconds);
		}
	}
}
