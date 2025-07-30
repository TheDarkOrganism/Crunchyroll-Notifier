namespace WinUIApp.Converters
{
	internal sealed class InterfaceConverter : JsonConverter<object?>
	{
		private static readonly ModelSerializerContext _context = ModelSerializerContext.Writing;

		public override bool CanConvert(Type typeToConvert)
		{
			return typeToConvert.IsInterface;
		}

		public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
		{
			if (value is null)
			{
				writer.WriteNullValue();

				return;
			}

			Type valueType = value.GetType();

			if (_context.GetTypeInfo(valueType) is not JsonTypeInfo jsonTypeInfo)
			{
				return;
			}

			writer.WriteStartObject();

			foreach (JsonPropertyInfo jsonProperty in jsonTypeInfo.Properties)
			{
				Func<object, object?>? getter = jsonProperty.Get;

				if (getter is null || jsonProperty.DeclaringType != valueType)
				{
					continue;
				}

				writer.WritePropertyName(jsonProperty.Name);

				JsonSerializer.Serialize(writer, getter(value), jsonProperty.PropertyType, _context);
			}

			writer.WriteEndObject();
		}
	}
}
