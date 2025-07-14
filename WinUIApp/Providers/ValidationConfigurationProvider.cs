namespace WinUIApp.Providers
{
	internal sealed class ValidationConfigurationProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : ConfigurationProvider
		where T : notnull
	{
		private static readonly Dictionary<string, ValidationAttribute[]> _attributePairs = typeof(T).GetValidationAttributes(static property => property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name);

		private readonly string _file;
		private readonly IFileProvider _fileProvider;
		private readonly ILogger<ValidationConfigurationProvider<T>> _logger;

		public ValidationConfigurationProvider(string file, IFileProvider fileProvider, ILogger<ValidationConfigurationProvider<T>> logger)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));
			ArgumentNullException.ThrowIfNull(fileProvider, nameof(fileProvider));
			ArgumentNullException.ThrowIfNull(logger, nameof(logger));

			_file = file;
			_fileProvider = fileProvider;
			_logger = logger;
		}

		private void WriteValue<TValue>(string? key, TValue? value)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return;
			}

			Set(key, value is string str ? str : value?.ToString());
		}

		private static bool TryParse<TValue>(JsonElement jsonElement, [NotNullWhen(true)] out TValue? value)
		{
			value = jsonElement.ValueKind switch
			{
				JsonValueKind.Undefined => default,
				JsonValueKind.Object => default,
				JsonValueKind.Array => default,
				JsonValueKind.String => jsonElement.GetString() is TValue v ? v : default,
				JsonValueKind.Number => jsonElement.GetDouble() is TValue v ? v : default,
				JsonValueKind.True => jsonElement.GetBoolean() is TValue v ? v : default,
				JsonValueKind.False => jsonElement.GetBoolean() is TValue v ? v : default,
				JsonValueKind.Null => default,
				_ => throw new NotImplementedException()
			};

			return value is not null;
		}

		private void ParseValue(JsonElement jsonElement, string? key)
		{
			switch (jsonElement.ValueKind)
			{
				case JsonValueKind.Undefined:
					break;
				case JsonValueKind.Object:
					foreach (JsonProperty jsonProperty in jsonElement.EnumerateObject())
					{
						string name = jsonProperty.Name;

						string subKey = key is null ? name : $"{key}:{name}";

						JsonElement value = jsonProperty.Value;

						ParseValue(value, subKey);

						if (_attributePairs.TryGetValue(name, out ValidationAttribute[]? attributes))
						{
							foreach (ValidationAttribute attribute in attributes)
							{
								switch (attribute)
								{
									case EnumDataTypeAttribute enumDataTypeAttribute:
										if (!TryParse(value, out string? stringValue) || !enumDataTypeAttribute.IsValid(stringValue))
										{
											Type enumType = enumDataTypeAttribute.EnumType;

											if (Enum.TryParse(enumType, stringValue, true, out object? result))
											{
												WriteValue(subKey, result);
											}
											else
											{
												Set(subKey, Enum.GetNames(enumType)[0]);
											}
										}
										break;
									case RangeAttribute rangeAttribute:
										if (!TryParse(value, out double number))
										{
											WriteValue(subKey, rangeAttribute.Maximum);

											continue;
										}

										if (!rangeAttribute.IsValid(number) && double.TryParse(rangeAttribute.Minimum.ToString(), out double min) && double.TryParse(rangeAttribute.Maximum.ToString(), out double max))
										{
											WriteValue(subKey, Math.Clamp(number, min, max));
										}
										break;
									case TimeSpanRangeAttribute timeSpanRangeAttribute:
										if (TryParse(value, out stringValue) && TimeSpan.TryParse(stringValue, out TimeSpan timeSpan) && !timeSpanRangeAttribute.IsValid(timeSpan))
										{
											WriteValue(subKey, timeSpan.Clamp(timeSpanRangeAttribute.Mininum, timeSpanRangeAttribute.Maxinum));
										}
										break;
									default:
										break;
								}
							}
						}
					}
					break;
				case JsonValueKind.Array:
					int index = 0;

					foreach (JsonElement element in jsonElement.EnumerateArray())
					{
						ParseValue(element, $"{key ?? "Array"}:{index}");

						index++;
					}
					break;
				case JsonValueKind.String:
					WriteValue(key, jsonElement.GetString());
					break;
				case JsonValueKind.Number:
					WriteValue(key, jsonElement.GetDouble());
					break;
				case JsonValueKind.True:
					WriteValue(key, bool.TrueString);
					break;
				case JsonValueKind.False:
					WriteValue(key, bool.FalseString);
					break;
				case JsonValueKind.Null:
					WriteValue(key, default(string));
					break;
				default:
					break;
			}
		}

		public override void Load()
		{
			IFileInfo fileInfo = _fileProvider.GetFileInfo(_file);

			if (fileInfo.Exists)
			{
				using Stream stream = fileInfo.CreateReadStream();

				try
				{
					using JsonDocument jsonDocument = JsonDocument.Parse(stream);

					ParseValue(jsonDocument.RootElement, null);
				}
				catch (JsonException ex)
				{
					_logger.LogWarning(ex, "Failed to read json from {File}.", _file);

					ResourceHelper.RestoreFile(_file);
				}
				catch (Exception ex)
				{
					_logger.LogCritical(ex, "Unable to read {File}.", _file);

					Environment.Exit(13);
				}
			}
		}
	}
}
