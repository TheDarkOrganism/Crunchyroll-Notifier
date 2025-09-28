namespace WinUIApp.Providers
{
	internal sealed class ValidationConfigurationProvider<TModel, TIModel> : ConfigurationProvider
		where TModel : ModelBase, TIModel
		where TIModel : class, IModelBase
	{
		private static readonly Dictionary<string, ValidationAttribute[]> _attributePairs = typeof(TModel).GetValidationAttributes();

		private readonly IFileModel<TIModel> _fileModel;
		private readonly IFileProvider _fileProvider;
		private readonly ILogger<ValidationConfigurationProvider<TModel, TIModel>> _logger;

		public ValidationConfigurationProvider(IFileModel<TIModel> fileModel, IFileProvider fileProvider, ILogger<ValidationConfigurationProvider<TModel, TIModel>> logger)
		{
			ArgumentNullException.ThrowIfNull(fileModel, nameof(fileModel));
			ArgumentNullException.ThrowIfNull(fileProvider, nameof(fileProvider));
			ArgumentNullException.ThrowIfNull(logger, nameof(logger));

			_fileModel = fileModel;
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

		private static bool TryParseEnum(JsonElement jsonElement, EnumDataTypeAttribute enumDataTypeAttribute, [NotNullWhen(true)] out object? enumValue)
		{
			enumValue = null;

			return TryParse(jsonElement, out string? stringValue) && enumDataTypeAttribute.IsValid(stringValue) && Enum.TryParse(enumDataTypeAttribute.EnumType, stringValue, true, out enumValue);
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

						if (_attributePairs?.TryGetValue(name, out ValidationAttribute[]? attributes) is true)
						{
							foreach (ValidationAttribute attribute in attributes)
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

										if (attributes.OfType<EnumDataTypeAttribute>().FirstOrDefault() is EnumDataTypeAttribute enumDataType && TryParseEnum(value, enumDataType, out enumValue) && allowedValuesAttribute.IsValid(enumValue))
										{
											continue;
										}

										if (TryParse(value, out object? objectValue) && allowedValuesAttribute.IsValid(objectValue))
										{
											continue;
										}

										WriteValue(subKey, values[0]);
										
										break;
									case EnumDataTypeAttribute enumDataTypeAttribute:
										if (TryParseEnum(value, enumDataTypeAttribute, out enumValue))
										{
											WriteValue(subKey, enumValue);
										}
										else
										{
											Set(subKey, Enum.GetNames(enumDataTypeAttribute.EnumType)[0]);
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
										if (TryParse(value, out string? stringValue) && TimeSpan.TryParse(stringValue, out TimeSpan timeSpan) && !timeSpanRangeAttribute.IsValid(timeSpan))
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
			string file = _fileModel.File;

			IFileInfo fileInfo = _fileProvider.GetFileInfo(file);

			if (fileInfo.Exists)
			{
				try
				{
					_fileModel.Wait(() =>
					{
						using Stream stream = fileInfo.CreateReadStream();

						using JsonDocument jsonDocument = JsonDocument.Parse(stream, ModelSerializerContext.DocumentOptions);

						ParseValue(jsonDocument.RootElement, null);
					}, _logger);
				}
				catch (JsonException ex)
				{
					_logger.LogWarning(ex, "Failed to read json from {File}.", file);
				}
				catch (Exception ex)
				{
					_logger.LogCritical(ex, "Unable to read {File}.", file);

					Environment.Exit(13);
				}
			}
		}
	}
}
