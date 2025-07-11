namespace WinUIApp.Providers
{
	internal sealed class ValidationConfigurationProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : ConfigurationProvider
		where T : notnull
	{
		private static readonly Dictionary<string, ValidationAttribute[]> _attributePairs = typeof(T).GetValidationAttributes(static property => property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name);

		private readonly string _file;
		private readonly IFileProvider _fileProvider;

		public ValidationConfigurationProvider(string file, IFileProvider fileProvider)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));
			ArgumentNullException.ThrowIfNull(fileProvider, nameof(fileProvider));

			_file = file;
			_fileProvider = fileProvider;
		}

		private void WriteValue<TValue>(string? key, TValue? value)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return;
			}

			Data[key] = value is string str ? str : value?.ToString();
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

						string? stringValue = value.ValueKind is JsonValueKind.Null ? null : value.ToString();

						if (_attributePairs.TryGetValue(name, out ValidationAttribute[]? attributes))
						{
							foreach (ValidationAttribute attribute in attributes)
							{
								switch (attribute)
								{
									case EnumDataTypeAttribute enumDataTypeAttribute:
										if (stringValue is null || !enumDataTypeAttribute.IsValid(stringValue))
										{
											Type enumType = enumDataTypeAttribute.EnumType;

											if (Enum.TryParse(enumType, stringValue, true, out object? result))
											{
												WriteValue(subKey, result);
											}
											else
											{
												WriteValue(subKey, Enum.GetNames(enumType)[0]);
											}
										}
										break;
									case RangeAttribute rangeAttribute:
										if (stringValue is null)
										{
											WriteValue(subKey, rangeAttribute.Maximum);

											continue;
										}

										if (double.TryParse(stringValue, out double number) && !rangeAttribute.IsValid(number) && double.TryParse(rangeAttribute.Minimum.ToString(), out double min) && double.TryParse(rangeAttribute.Maximum.ToString(), out double max))
										{
											WriteValue(subKey, Math.Clamp(number, min, max));
										}
										break;
									case TimeSpanRangeAttribute timeSpanRangeAttribute:
										if (TimeSpan.TryParse(stringValue, out TimeSpan timeSpan) && !timeSpanRangeAttribute.IsValid(timeSpan))
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
					Debug.WriteLine(ex);

					ResourceHelper.RestoreFile(_file);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);

					Environment.Exit(13);
				}
			}
		}
	}
}
