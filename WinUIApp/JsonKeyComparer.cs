namespace WinUIApp
{
	internal sealed class JsonKeyComparer : IEqualityComparer<string>
	{
		private static readonly JsonNamingPolicy _jsonNamingPolicy = ModelSerializerContext.Default.Options.PropertyNamingPolicy!;

		public bool Equals(string? x, string? y)
		{
			return x == y || (x is not null && y is not null && x == _jsonNamingPolicy.ConvertName(y));
		}

		public int GetHashCode([DisallowNull] string obj)
		{
			return _jsonNamingPolicy.ConvertName(obj).GetHashCode();
		}
	}
}
