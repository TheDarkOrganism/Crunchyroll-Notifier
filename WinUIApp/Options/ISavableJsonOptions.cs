namespace WinUIApp.Options
{
	public interface ISavableJsonOptions<out TOptions> : IOptions<TOptions>
		where TOptions : class
	{
		void Save();

		Task SaveAsync();
	}
}
