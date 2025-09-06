namespace WinUIApp
{
	internal sealed class JsonKeyComparer : IEqualityComparer<string>
	{
		private static readonly ModelSerializerContext _serializerContext = ModelSerializerContext.Default;

		public bool Equals(string? x, string? y)
		{
			return x == y || (x is not null && y is not null && x == _serializerContext.GetJsonName(y));
		}

		public int GetHashCode([DisallowNull] string obj)
		{
			return _serializerContext.GetJsonName(obj).GetHashCode();
		}
	}
}
