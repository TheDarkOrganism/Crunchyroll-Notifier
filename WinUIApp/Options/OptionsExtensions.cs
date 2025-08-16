namespace WinUIApp.Options
{
	internal static class OptionsExtensions
	{
		private static bool TryParseList(Type type, object? value, [NotNullWhen(true)] out IList? list)
		{
			list = type.GenericTypeArguments.Length == 1 ? value as IList : null;

			return list is not null;
		}

		private static object? ParseValue(Type type, IConfigurationSection configurationSection, IConfigurationSection childSection, object? fallbackValue, bool canWrite)
		{
			object? altFallbackValue = fallbackValue;

			if (!canWrite)
			{
				if (!TryParseList(type, fallbackValue, out _))
				{
					return fallbackValue;
				}

				altFallbackValue = null;
			}

			if (!childSection.Exists())
			{
				return altFallbackValue;
			}

			if (TryParseList(type, fallbackValue, out IList? list))
			{
				int index = 0;

				IConfigurationSection arraySection;

				do
				{
					arraySection = configurationSection.GetSection($"{childSection.Key}:{index}");

					if (ParseValue(type.GenericTypeArguments[0], configurationSection, arraySection, null, true) is object arrayValue && list.Add(arrayValue) == -1)
					{
						break;
					}

					index++;
				} while (arraySection.Exists());

				return altFallbackValue;
			}

			string? value = childSection.Value;

			if (type.IsEnum)
			{
				return Enum.TryParse(type, value, true, out object? enumValue) ? enumValue : fallbackValue;
			}

			switch (type.Name)
			{
				case nameof(DateTime):
					return DateTime.TryParse(value, out DateTime dateTime) ? dateTime : fallbackValue;
				case nameof(TimeSpan):
					return TimeSpan.TryParse(value, out TimeSpan timeSpan) ? timeSpan : fallbackValue;
				default:
					if (value is null && (type.IsValueType || Nullable.GetUnderlyingType(type) is null))
					{
						return fallbackValue;
					}

					try
					{
						return Convert.ChangeType(value, type) ?? fallbackValue;
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex);

						return fallbackValue;
					}
			}
		}

		public static IHostBuilder ConfigureSavableJson<TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TIOptions>(this IHostBuilder hostBuilder, FileModel<TOptions, TIOptions> fileModel)
			where TOptions : notnull, ModelBase, TIOptions, new()
			where TIOptions : class, IModelBase
		{
			return hostBuilder.ConfigureServices((context, services) =>
			{
				_ = services.AddSingleton<IOptions<TIOptions>>(_ =>
				{
					TOptions options = new();

					IConfigurationSection configurationSection = context.Configuration.GetSection(fileModel.ConfigurationSection);

					if (configurationSection.Exists())
					{
						foreach (JsonPropertyInfo propertyInfo in typeof(TOptions).GetDeclaredProperties())
						{
							Action<object, object?>? setter = propertyInfo.Set;

							if (ParseValue(propertyInfo.PropertyType, configurationSection, configurationSection.GetSection(propertyInfo.Name), propertyInfo.Get?.Invoke(options), setter is not null) is object value)
							{
								setter?.Invoke(options, value);
							}
						}
					}

					return new Options<TOptions>(options);
				});

				_ = services.AddSingleton<ISavableJsonOptions<TIOptions>, SavableJsonOptions<TIOptions>>();
			});
		}
	}
}
