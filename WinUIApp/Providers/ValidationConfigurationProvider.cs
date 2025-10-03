namespace WinUIApp.Providers
{
	internal sealed partial class ValidationConfigurationProvider<TModel> : JsonProviderBase<ValidationConfigurationProvider<TModel>>
		where TModel : notnull, ModelBase
	{
		private static readonly ModelSerializerContext _serializerContext = ModelSerializerContext.Default;

		private static readonly JsonSerializerOptions _jsonSerializerOptions = _serializerContext.Options;

		private static readonly JsonReaderOptions _jsonReaderOptions = new()
		{
			AllowTrailingCommas = _jsonSerializerOptions.AllowTrailingCommas,
			CommentHandling = _jsonSerializerOptions.ReadCommentHandling,
			MaxDepth = _jsonSerializerOptions.MaxDepth
		};

		private readonly string _section;

		public ValidationConfigurationProvider(JsonConfigurationSource jsonConfigurationSource, string section, ILogger<ValidationConfigurationProvider<TModel>> logger) : base(jsonConfigurationSource, logger)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(section, nameof(section));

			_section = section;
		}

		private void Set(string? key, object? value, JsonTypeInfo jsonTypeInfo)
		{
			if (string.IsNullOrWhiteSpace(key) || (value is null && Nullable.GetUnderlyingType(jsonTypeInfo.Type) is null))
			{
				return;
			}

			Set(key, value is string str ? str : value?.ToString());
		}

		private bool TryRead(ref Utf8JsonReader utf8JsonReader)
		{
			try
			{
				return utf8JsonReader.Read();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to read the JSON file {File}.", Source.Path);

				return false;
			}
		}

		private object? ParseNumber(ref Utf8JsonReader utf8JsonReader, JsonTypeInfo? jsonTypeInfo)
		{
			if (utf8JsonReader.TokenType != JsonTokenType.Number)
			{
				Logger.LogDebug("The current JSON token is not a number.");

				return null;
			}

			if (jsonTypeInfo is null)
			{
				return utf8JsonReader.GetDouble();
			}
			else
			{
				try
				{
					return JsonSerializer.Deserialize(ref utf8JsonReader, jsonTypeInfo);
				}
				catch (JsonException ex)
				{
					Logger.LogWarning(ex, "Unable to desrialize the JSON token as {Type}.", jsonTypeInfo.Type.Name);
				}
				catch (Exception ex)
				{
					Logger.LogWarning(ex, "Unable to desrialize the JSON token.");
				}

				return null;
			}
		}

		private bool TryParse<TValue>(ref Utf8JsonReader utf8JsonReader, [NotNullWhen(true)] out TValue? value)
			where TValue : notnull
		{
			Type valueType = typeof(TValue);

			value = (utf8JsonReader.TokenType switch
			{
				JsonTokenType.None => null,
				JsonTokenType.StartObject => null,
				JsonTokenType.EndObject => null,
				JsonTokenType.StartArray => null,
				JsonTokenType.EndArray => null,
				JsonTokenType.PropertyName => utf8JsonReader.GetString(),
				JsonTokenType.Comment => utf8JsonReader.GetString(),
				JsonTokenType.String => utf8JsonReader.GetString(),
				JsonTokenType.Number => ParseNumber(ref utf8JsonReader, _serializerContext.GetTypeInfo(valueType)),
				JsonTokenType.True => true,
				JsonTokenType.False => false,
				JsonTokenType.Null => null,
				_ => null,
			}) is TValue v ? v : default;

			if (value is null)
			{
				Logger.LogWarning("Unable to parse JSON token as {Type}.", valueType.Name);

				return false;
			}

			return true;
		}

		private bool TryParseEnum(ref Utf8JsonReader utf8JsonReader, EnumDataTypeAttribute enumDataTypeAttribute, [NotNullWhen(true)] out object? enumValue)
		{
			enumValue = null;

			return TryParse(ref utf8JsonReader, out string? stringValue) && enumDataTypeAttribute.IsValid(stringValue) && Enum.TryParse(enumDataTypeAttribute.EnumType, stringValue, out enumValue);
		}

		private void ParseValue(ref Utf8JsonReader utf8JsonReader, JsonTypeInfo jsonTypeInfo, string? key)
		{
			if (!TryRead(ref utf8JsonReader))
			{
				Logger.LogTrace("Parsing finished.");

				return;
			}

			ParseToken(ref utf8JsonReader, jsonTypeInfo, key);

			ParseValue(ref utf8JsonReader, jsonTypeInfo, key);
		}

		private void ParseToken(ref Utf8JsonReader utf8JsonReader, JsonTypeInfo jsonTypeInfo, string? key)
		{
			switch (utf8JsonReader.TokenType)
			{
				case JsonTokenType.None:
					break;
				case JsonTokenType.StartObject:
					break;
				case JsonTokenType.EndObject:
					break;
				case JsonTokenType.StartArray:
					if (_serializerContext.TryGetJsonTypeInfo(jsonTypeInfo.ElementType, out JsonTypeInfo? typeInfo))
					{
						int index = 0;

						do
						{
							ParseValue(ref utf8JsonReader, typeInfo, $"{key ?? _section}:{index}");

							index++;
						} while (TryRead(ref utf8JsonReader) && utf8JsonReader.TokenType != JsonTokenType.EndArray);
					}
					break;
				case JsonTokenType.EndArray:
					break;
				case JsonTokenType.PropertyName:
					string? propertyName = utf8JsonReader.GetString();

					if (string.IsNullOrEmpty(propertyName))
					{
						break;
					}

					if (string.IsNullOrEmpty(key))
					{
						if (propertyName == _section && !Data.ContainsKey(propertyName))
						{
							ParseValue(ref utf8JsonReader, jsonTypeInfo, propertyName);

							Logger.LogTrace("Parsed property {PropertyName}.", propertyName);

							return;
						}
						else
						{
							if (utf8JsonReader.TrySkip())
							{
								Logger.LogTrace("Skipped property {PropertyName}.", propertyName);
							}

							break;
						}
					}

					string subKey = $"{key}:{propertyName}";

					if (jsonTypeInfo?.Properties.FirstOrDefault(p => p.Name == propertyName) is JsonPropertyInfo jsonPropertyInfo && _serializerContext.TryGetJsonTypeInfo(jsonPropertyInfo.PropertyType, out JsonTypeInfo? jsonType) && TryRead(ref utf8JsonReader))
					{
						ParseToken(ref utf8JsonReader, jsonType, subKey);

						if (jsonPropertyInfo.AttributeProvider?.GetCustomAttributes(typeof(ValidationAttribute), false) is object[] attributes)
						{
							const string logFormat = "Failed to validate {Attribute} with {Type} for {PropertyName}.";

							const string rangeLogFormat = "The {Attribute} with {Value} must be between {Min} and {Max} for {PropertyName}.";

							foreach (object attribute in attributes)
							{
								object? enumValue;

								switch (attribute)
								{
									case AllowedValuesAttribute allowedValuesAttribute:
										object?[] values = allowedValuesAttribute.Values;

										if (values.Length == 0)
										{
											continue;
										}

										string? enumName = null;

										if (attributes.OfType<EnumDataTypeAttribute>().FirstOrDefault() is EnumDataTypeAttribute enumDataType)
										{
											enumName = enumDataType.EnumType.Name;

											if (TryParseEnum(ref utf8JsonReader, enumDataType, out enumValue) && allowedValuesAttribute.IsValid(enumValue))
											{
												continue;
											}
										}

										if (TryParse(ref utf8JsonReader, out object? objectValue) && allowedValuesAttribute.IsValid(objectValue))
										{
											continue;
										}

										Logger.LogWarning(logFormat, nameof(AllowedValuesAttribute), enumName ?? nameof(Object), propertyName);

										Set(subKey, values[0], jsonTypeInfo);

										break;
									case EnumDataTypeAttribute enumDataTypeAttribute:
										if (TryParseEnum(ref utf8JsonReader, enumDataTypeAttribute, out enumValue))
										{
											Set(subKey, enumValue, jsonTypeInfo);
										}
										else
										{
											Type enumType = enumDataTypeAttribute.EnumType;

											Logger.LogWarning(logFormat, nameof(EnumDataTypeAttribute), enumType.Name, propertyName);

											Set(subKey, Enum.GetNames(enumType)[0]);
										}
										break;
									case RangeAttribute rangeAttribute:
										object minValue = rangeAttribute.Minimum;
										object maxValue = rangeAttribute.Maximum;

										if (!TryParse(ref utf8JsonReader, out double number))
										{
											Logger.LogWarning(rangeLogFormat, nameof(RangeAttribute), 0, minValue, maxValue, propertyName);

											Set(subKey, maxValue, jsonTypeInfo);

											continue;
										}

										if (!rangeAttribute.IsValid(number) && double.TryParse(minValue.ToString(), out double min) && double.TryParse(maxValue.ToString(), out double max))
										{
											Logger.LogWarning(rangeLogFormat, nameof(RangeAttribute), number, min, max, propertyName);

											Set(subKey, Math.Clamp(number, min, max), jsonTypeInfo);
										}
										break;
									case TimeSpanRangeAttribute timeSpanRangeAttribute:
										TimeSpan mininum = timeSpanRangeAttribute.Mininum;
										TimeSpan maxinum = timeSpanRangeAttribute.Maxinum;

										if (!TryParse(ref utf8JsonReader, out string? stringValue) || !TimeSpan.TryParse(stringValue, out TimeSpan timeSpan))
										{
											Logger.LogWarning(rangeLogFormat, nameof(TimeSpanRangeAttribute), TimeSpan.Zero, mininum, maxinum, propertyName);

											Set(subKey, mininum, jsonTypeInfo);

											continue;
										}

										if (!timeSpanRangeAttribute.IsValid(timeSpan))
										{
											Logger.LogWarning(rangeLogFormat, nameof(TimeSpanRangeAttribute), timeSpan, mininum, maxinum, propertyName);

											Set(subKey, timeSpan.Clamp(mininum, maxinum), jsonTypeInfo);
										}

										break;
									default:
										break;
								}
							}
						}
					}

					break;
				case JsonTokenType.Comment:
					Set(key, utf8JsonReader.GetString(), jsonTypeInfo);
					break;
				case JsonTokenType.String:
					Set(key, utf8JsonReader.GetString(), jsonTypeInfo);
					break;
				case JsonTokenType.Number:
					Set(key, ParseNumber(ref utf8JsonReader, jsonTypeInfo), jsonTypeInfo);
					break;
				case JsonTokenType.True:
					Set(key, bool.TrueString, jsonTypeInfo);
					break;
				case JsonTokenType.False:
					Set(key, bool.FalseString, jsonTypeInfo);
					break;
				case JsonTokenType.Null:
					Set(key, null, jsonTypeInfo);
					break;
				default:
					break;
			}
		}

		public override void Load(Stream stream)
		{
			if (stream.Length <= 2)
			{
				return;
			}

			string? file = Source.Path;

			Logger.LogTrace("Parsing JSON configuration from {File}.", file);

			if (_serializerContext.TryGetJsonTypeInfo(typeof(TModel), out JsonTypeInfo? jsonTypeInfo))
			{
				using MemoryStream memoryStream = new();

				stream.CopyTo(memoryStream);

				Utf8JsonReader utf8JsonReader = new(memoryStream.ToArray(), _jsonReaderOptions);

				ParseValue(ref utf8JsonReader, jsonTypeInfo, null);
			}
			else
			{
				base.Load(stream);
			}
		}
	}
}
