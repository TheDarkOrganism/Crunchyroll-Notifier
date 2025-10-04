namespace WinUIApp.Validators
{
	internal abstract class SingleInstanceBase<TInstance>
		where TInstance : notnull, SingleInstanceBase<TInstance>, new()
	{
		public static TInstance Instance { get; } = new();

		protected SingleInstanceBase()
		{
			if (Instance is not null)
			{
				throw new InvalidOperationException($"This {typeof(TInstance)} should only be accessed using the static {nameof(Instance)} property.");
			}
		}
	}
}
