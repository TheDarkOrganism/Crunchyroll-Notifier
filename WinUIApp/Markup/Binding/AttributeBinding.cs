using WinUIApp.Markup.Converters;

namespace WinUIApp.Markup.Binding
{
	[MarkupExtensionReturnType(ReturnType = typeof(string))]
	internal sealed partial class AttributeBinding : MarkupExtension
	{
		[DisallowNull]
		public PropertyPath? Property { get; set; }

		[DisallowNull]
		public IAttributeConverter? Converter { get; set; }

		private Type? _dataContext;

		public Type? DataContext
		{
			get => _dataContext ?? Converter?.DataContext;
			set => _dataContext = value;
		}

		protected override object ProvideValue(IXamlServiceProvider serviceProvider)
		{
			if (DataContext is null)
			{
				if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget valueTarget && valueTarget.TargetObject is FrameworkElement frameworkElement)
				{
					DataContext = frameworkElement.DataContext?.GetType();
				}

				if (DataContext is null)
				{
					throw new InvalidOperationException($"{nameof(DataContext)} is null");
				}
			}

			string? path = Property?.Path;

			if (string.IsNullOrWhiteSpace(path))
			{
				throw new InvalidOperationException($"{nameof(Property)} is null or white space.");
			}

			if (Converter is null)
			{
				throw new InvalidOperationException($"{nameof(Converter)} is null.");
			}

			ModelSerializerContext modelContext = ModelSerializerContext.Default;

			if (!modelContext.TryGetJsonTypeInfo(DataContext, out JsonTypeInfo? jsonTypeInfo))
			{
				throw new InvalidOperationException($"No metadata found for {DataContext}.");
			}

			if (!jsonTypeInfo.TryGetJsonPropertyInfo(modelContext.GetJsonName(path), out JsonPropertyInfo? jsonPropertyInfo))
			{
				throw new InvalidOperationException($"The property path {Property} was not found.");
			}

			return jsonPropertyInfo.TryGetAttributes(out IEnumerable<Attribute>? attributes) ? Converter.Convert(attributes, path) : path;
		}
	}
}
