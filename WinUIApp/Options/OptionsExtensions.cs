namespace WinUIApp.Options
{
	internal static class OptionsExtensions
	{
		public static IHostBuilder ConfigureSavableJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TOptions>(this IHostBuilder hostBuilder, string file, string? section = null)
			where TOptions : ModelBase, new()
		{
			return hostBuilder.ConfigureServices((context, services) =>
			{
				section ??= Path.GetFileNameWithoutExtension(file);

				_ = services.AddSingleton<IOptions<TOptions>>(_ =>
				{
					TOptions options = new();

					IConfigurationSection configurationSection = context.Configuration.GetSection(section);

					if (configurationSection.Exists())
					{
						foreach (PropertyInfo propertyInfo in typeof(TOptions).GetProperties())
						{
							if (propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
							{
								continue;
							}

							IConfigurationSection childSection = configurationSection.GetSection(propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? propertyInfo.Name);

							if (!childSection.Exists())
							{
								continue;
							}

							Type propertyType = propertyInfo.PropertyType;

							string? value = childSection.Value;

							object? currentValue = propertyInfo.GetValue(options);

							if (propertyType.IsEnum)
							{
								propertyInfo.SetValue(options, Enum.TryParse(propertyType, value, true, out object? result) ? result : currentValue);

								continue;
							}

							Type[] typeArguments = propertyType.GenericTypeArguments;

							if (typeArguments.Length == 1 && currentValue is IList list)
							{
								int index = 0;

								IConfigurationSection arraySection;

								do
								{
									arraySection = configurationSection.GetSection($"{childSection.Key}:{index}");

									if (arraySection.Exists() && list.Add(Convert.ChangeType(arraySection.Value, typeArguments[0])) == -1)
									{
										break;
									}

									index++;
								} while (arraySection.Exists());

								continue;
							}

							propertyInfo.SetValue(options, propertyType.Name switch
							{
								nameof(DateTime) => DateTime.TryParse(value, out DateTime result) ? result : currentValue,
								nameof(TimeSpan) => TimeSpan.TryParse(value, out TimeSpan result) ? result : currentValue,
								_ => Convert.ChangeType(childSection.Value, propertyType)
							} ?? currentValue);
						}
					}

					return new Options<TOptions>(options);
				});

				_ = services.AddSingleton<ISavableJsonOptions<TOptions>>(provider => new SavableJsonOptions<TOptions>(file, section, context.HostingEnvironment.ContentRootFileProvider, provider.GetRequiredService<IOptions<TOptions>>()));
			});
		}
	}
}
